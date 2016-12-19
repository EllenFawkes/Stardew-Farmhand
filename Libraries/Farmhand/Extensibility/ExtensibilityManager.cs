﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Farmhand.Logging;
using Newtonsoft.Json;
using StardewValley;

namespace Farmhand.Extensibility
{
    public class ExtensibilityManager
    {
        internal static List<FarmhandExtension> Extensions { get; set; } = new List<FarmhandExtension>();

        internal static void TryLoadExtensions()
        {
            Log.Verbose("Loading Compatibility Layers");
            var appRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (appRoot == null)
                throw new NullReferenceException("appRoot was null");

            var extensions = Directory.GetFiles(Path.Combine(appRoot, Constants.ExtensionsDirectory), "manifest.json", SearchOption.AllDirectories);

            foreach (var extension in extensions)
            {
                try
                {
                    Log.Verbose($"Loading Extension Manifest");
                    ExtensionManifest manifest;
                    using (var r = new StreamReader(extension))
                    {
                        var json = r.ReadToEnd();
                        manifest = JsonConvert.DeserializeObject<ExtensionManifest>(json);
                    }

                    if (manifest.LoadOnlyIfModsPresent)
                    {
                        if (!AreModsPresent(manifest.ModsFolder))
                        {
                            Log.Verbose($"No mods detected for {manifest.Name} - skipping");
                            continue;
                        }
                    }

                    var extensionRoot = Path.GetDirectoryName(extension);
                    if (extensionRoot == null)
                    {
                        Log.Error($"Path.GetDirectoryName(extension) returned null!");
                        return;
                    }

                    var extensionDll = Path.Combine(extensionRoot, manifest.ExtensionDll) + ".dll";
                    var assm = Assembly.LoadFrom(extensionDll);
                    if (assm.GetTypes().Count(x => x.BaseType == typeof(FarmhandExtension)) <= 0) continue;

                    var type = assm.GetTypes().First(x => x.BaseType == typeof(FarmhandExtension));
                    var inst = (FarmhandExtension)assm.CreateInstance(type.ToString());
                    if (inst == null) continue;

                    inst.OwnAssembly = assm;
                    inst.Manifest = manifest;

                    Extensions.Add(inst);
                    inst.RootDirectory = appRoot;
                    inst.Initialise();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    var test = ex.LoaderExceptions;
                    Log.Exception("Failed to load extension: " + test[0].Message, ex);
                }
            }
        }

        private static bool AreModsPresent(string manifestModsFolder)
        {
            var mods = Directory.GetFiles(Path.Combine(Constants.DefaultModPath, manifestModsFolder), "*.json", SearchOption.AllDirectories);
            return mods.Any();
        }

        public static void LoadMods(string modsPath)
        {
            foreach (var ext in Extensions)
            {
                ext.LoadMods(modsPath);
            }
        }

        public static void SetGameInstance(Game1 game1)
        {
            foreach (var extensions in Extensions)
            {
                extensions.GameInstance = game1;
            }
        }
    }
}
