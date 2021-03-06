﻿namespace Farmhand.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Farmhand.Attributes;
    using Farmhand.Events;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using StardewValley;
    using StardewValley.BellsAndWhistles;
    using StardewValley.Locations;
    using StardewValley.Menus;
    using StardewValley.Tools;

    using xTile.Dimensions;

    using Rectangle = Microsoft.Xna.Framework.Rectangle;



    /// <summary>
    ///     Overrides Stardew's Game1, allowing for advanced callback events to be added
    /// </summary>
    [HookRedirectConstructorFromBase("Farmhand.API.Game", "GetFarmhandOverrideInstance", new Type[] { })]
    public class Game1 : StardewValley.Game1
    {
        internal readonly Dictionary<string, Action<GameTime>> VersionSpecificOverrides =
            new Dictionary<string, Action<GameTime>>();

       // internal bool ZoomLevelIsOne => options.zoomLevel.Equals(1.0f);
        internal bool ZoomLevelIsOne => false;

        /// <summary>
        ///     Main Draw loop.
        /// </summary>
        /// <param name="gameTime">
        ///     The elapsed game time.
        /// </param>
        protected override void Draw(GameTime gameTime)
        {
            if (Constants.OverrideGameDraw)
            {
                GraphicsEvents.OnBeforeDraw(this);
                if (this.VersionSpecificOverrides.ContainsKey(version))
                {
                    this.VersionSpecificOverrides[version](gameTime);
                }
                else
                {
                    this.DefaultDraw(gameTime);
                }

                GraphicsEvents.OnAfterDraw(this);
            }
            else
            {
                base.Draw(gameTime);
            }
        }

        private void DefaultDraw(GameTime gameTime)
        {
            if (!this.ZoomLevelIsOne)
            {
                this.GraphicsDevice.SetRenderTarget(this.screen);
            }

            this.GraphicsDevice.Clear(this.bgColor);
            if (options.showMenuBackground && Farmhand.API.Game.ActiveClickableMenu != null
                && Farmhand.API.Game.ActiveClickableMenu.showWithoutTransparencyIfOptionIsSet())
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                Farmhand.API.Game.ActiveClickableMenu.drawBackground(spriteBatch);
                GraphicsEvents.OnPreRenderGuiEvent(this, spriteBatch, gameTime, this.screen);
                Farmhand.API.Game.ActiveClickableMenu.draw(spriteBatch);
                GraphicsEvents.OnPostRenderGuiEvent(this, spriteBatch, gameTime, this.screen);
                spriteBatch.End();
                if (this.ZoomLevelIsOne)
                {
                    return;
                }

                this.GraphicsDevice.SetRenderTarget(null);
                this.GraphicsDevice.Clear(this.bgColor);
                spriteBatch.Begin(
                    SpriteSortMode.Immediate,
                    BlendState.AlphaBlend,
                    SamplerState.LinearClamp,
                    DepthStencilState.Default,
                    RasterizerState.CullNone);
                spriteBatch.Draw(
                    this.screen,
                    Vector2.Zero,
                    this.screen.Bounds,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    options.zoomLevel,
                    SpriteEffects.None,
                    1f);
                spriteBatch.End();
                return;
            }

            if (gameMode == 11)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                spriteBatch.DrawString(
                    smoothFont,
                    "Stardew Valley has crashed...",
                    new Vector2(16f, 16f),
                    Color.HotPink);
                spriteBatch.DrawString(
                    smoothFont,
                    "Please send the error report or a screenshot of this message to @ConcernedApe. (http://stardewvalley.net/contact/)",
                    new Vector2(16f, 32f),
                    new Color(0, 255, 0));
                spriteBatch.DrawString(
                    smoothFont,
                    parseText(errorMessage, smoothFont, Farmhand.API.Game.GraphicsDevice.Viewport.Width),
                    new Vector2(16f, 48f),
                    Color.White);
                spriteBatch.End();
                return;
            }

            if (currentMinigame != null)
            {
                currentMinigame.draw(spriteBatch);
                if (globalFade && !menuUp && (!nameSelectUp || messagePause))
                {
                    spriteBatch.Begin(
                        SpriteSortMode.Deferred,
                        BlendState.AlphaBlend,
                        SamplerState.PointClamp,
                        null,
                        null);
                    spriteBatch.Draw(
                        fadeToBlackRect,
                        Farmhand.API.Game.GraphicsDevice.Viewport.Bounds,
                        Color.Black * (gameMode == 0 ? 1f - fadeToBlackAlpha : fadeToBlackAlpha));
                    spriteBatch.End();
                }

                if (!this.ZoomLevelIsOne)
                {
                    this.GraphicsDevice.SetRenderTarget(null);
                    this.GraphicsDevice.Clear(this.bgColor);
                    spriteBatch.Begin(
                        SpriteSortMode.Immediate,
                        BlendState.AlphaBlend,
                        SamplerState.LinearClamp,
                        DepthStencilState.Default,
                        RasterizerState.CullNone);
                    spriteBatch.Draw(
                        this.screen,
                        Vector2.Zero,
                        this.screen.Bounds,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        options.zoomLevel,
                        SpriteEffects.None,
                        1f);
                    spriteBatch.End();
                }

                return;
            }

            if (showingEndOfNightStuff)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                Farmhand.API.Game.ActiveClickableMenu?.draw(spriteBatch);
                spriteBatch.End();
                if (this.ZoomLevelIsOne)
                {
                    return;
                }

                this.GraphicsDevice.SetRenderTarget(null);
                this.GraphicsDevice.Clear(this.bgColor);
                spriteBatch.Begin(
                    SpriteSortMode.Immediate,
                    BlendState.AlphaBlend,
                    SamplerState.LinearClamp,
                    DepthStencilState.Default,
                    RasterizerState.CullNone);
                spriteBatch.Draw(
                    this.screen,
                    Vector2.Zero,
                    this.screen.Bounds,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    options.zoomLevel,
                    SpriteEffects.None,
                    1f);
                spriteBatch.End();
                return;
            }

            if (gameMode == 6)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                var text = string.Empty;
                var num = 0;
                while (num < gameTime.TotalGameTime.TotalMilliseconds % 999.0 / 333.0)
                {
                    text += ".";
                    num++;
                }

                SpriteText.drawString(
                    spriteBatch,
                    "Loading" + text,
                    64,
                    Farmhand.API.Game.GraphicsDevice.Viewport.Height - 64,
                    999,
                    -1,
                    999,
                    1f,
                    1f,
                    false,
                    0,
                    "Loading...");
                spriteBatch.End();
                if (!this.ZoomLevelIsOne)
                {
                    this.GraphicsDevice.SetRenderTarget(null);
                    this.GraphicsDevice.Clear(this.bgColor);
                    spriteBatch.Begin(
                        SpriteSortMode.Immediate,
                        BlendState.AlphaBlend,
                        SamplerState.LinearClamp,
                        DepthStencilState.Default,
                        RasterizerState.CullNone);
                    spriteBatch.Draw(
                        this.screen,
                        Vector2.Zero,
                        this.screen.Bounds,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        options.zoomLevel,
                        SpriteEffects.None,
                        1f);
                    spriteBatch.End();
                }

                return;
            }

            if (gameMode == 0)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            }
            else
            {
                if (drawLighting)
                {
                    this.GraphicsDevice.SetRenderTarget(lightmap);
                    this.GraphicsDevice.Clear(Color.White * 0f);
                    spriteBatch.Begin(
                        SpriteSortMode.Deferred,
                        BlendState.NonPremultiplied,
                        SamplerState.PointClamp,
                        null,
                        null);
                    spriteBatch.Draw(
                        staminaRect,
                        lightmap.Bounds,
                        currentLocation.name.Equals("UndergroundMine")
                            ? mine.getLightingColor(gameTime)
                            : (!ambientLight.Equals(Color.White) && (!isRaining || !currentLocation.isOutdoors)
                                   ? ambientLight
                                   : outdoorLight));
                    for (var i = 0; i < currentLightSources.Count; i++)
                    {
                        if (Utility.isOnScreen(
                            currentLightSources.ElementAt(i).position,
                            (int)(currentLightSources.ElementAt(i).radius * tileSize * 4f)))
                        {
                            spriteBatch.Draw(
                                currentLightSources.ElementAt(i).lightTexture,
                                GlobalToLocal(viewport, currentLightSources.ElementAt(i).position)
                                / options.lightingQuality,
                                currentLightSources.ElementAt(i).lightTexture.Bounds,
                                currentLightSources.ElementAt(i).color,
                                0f,
                                new Vector2(
                                    currentLightSources.ElementAt(i).lightTexture.Bounds.Center.X,
                                    currentLightSources.ElementAt(i).lightTexture.Bounds.Center.Y),
                                currentLightSources.ElementAt(i).radius / options.lightingQuality,
                                SpriteEffects.None,
                                0.9f);
                        }
                    }

                    spriteBatch.End();
                    this.GraphicsDevice.SetRenderTarget(this.ZoomLevelIsOne ? null : this.screen);
                }

                if (bloomDay)
                {
                    bloom?.BeginDraw();
                }

                this.GraphicsDevice.Clear(this.bgColor);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                GraphicsEvents.OnPreRenderEvent(this, spriteBatch, gameTime, this.screen);
                background?.draw(spriteBatch);
                mapDisplayDevice.BeginScene(spriteBatch);
                currentLocation.Map.GetLayer("Back").Draw(mapDisplayDevice, viewport, Location.Origin, false, pixelZoom);
                currentLocation.drawWater(spriteBatch);
                if (CurrentEvent == null)
                {
                    using (var enumerator = currentLocation.characters.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            var current = enumerator.Current;
                            if (current != null && !current.swimming && !current.hideShadow && !current.IsMonster
                                && !currentLocation.shouldShadowBeDrawnAboveBuildingsLayer(current.getTileLocation()))
                            {
                                spriteBatch.Draw(
                                    shadowTexture,
                                    GlobalToLocal(
                                        viewport,
                                        current.position
                                        + new Vector2(
                                            current.sprite.spriteWidth * pixelZoom / 2f,
                                            current.GetBoundingBox().Height + (current.IsMonster ? 0 : pixelZoom * 3))),
                                    shadowTexture.Bounds,
                                    Color.White,
                                    0f,
                                    new Vector2(shadowTexture.Bounds.Center.X, shadowTexture.Bounds.Center.Y),
                                    (pixelZoom + current.yJumpOffset / 40f) * current.scale,
                                    SpriteEffects.None,
                                    Math.Max(0f, current.getStandingY() / 10000f) - 1E-06f);
                            }
                        }

                        goto IL_B30;
                    }
                }

                foreach (var current2 in CurrentEvent.actors)
                {
                    if (!current2.swimming && !current2.hideShadow
                        && !currentLocation.shouldShadowBeDrawnAboveBuildingsLayer(current2.getTileLocation()))
                    {
                        spriteBatch.Draw(
                            shadowTexture,
                            GlobalToLocal(
                                viewport,
                                current2.position
                                + new Vector2(
                                    current2.sprite.spriteWidth * pixelZoom / 2f,
                                    current2.GetBoundingBox().Height + (current2.IsMonster ? 0 : pixelZoom * 3))),
                            shadowTexture.Bounds,
                            Color.White,
                            0f,
                            new Vector2(shadowTexture.Bounds.Center.X, shadowTexture.Bounds.Center.Y),
                            (pixelZoom + current2.yJumpOffset / 40f) * current2.scale,
                            SpriteEffects.None,
                            Math.Max(0f, current2.getStandingY() / 10000f) - 1E-06f);
                    }
                }

                IL_B30:
                if (!Farmhand.API.Game.Player.swimming && !Farmhand.API.Game.Player.isRidingHorse()
                    && !currentLocation.shouldShadowBeDrawnAboveBuildingsLayer(Farmhand.API.Game.Player.getTileLocation()))
                {
                    spriteBatch.Draw(
                        shadowTexture,
                        GlobalToLocal(Farmhand.API.Game.Player.position + new Vector2(32f, 24f)),
                        shadowTexture.Bounds,
                        Color.White,
                        0f,
                        new Vector2(shadowTexture.Bounds.Center.X, shadowTexture.Bounds.Center.Y),
                        4f
                        - ((Farmhand.API.Game.Player.running || Farmhand.API.Game.Player.usingTool) && Farmhand.API.Game.Player.FarmerSprite.indexInCurrentAnimation > 1
                               ? Math.Abs(FarmerRenderer.featureYOffsetPerFrame[Farmhand.API.Game.Player.FarmerSprite.CurrentFrame])
                                 * 0.5f
                               : 0f),
                        SpriteEffects.None,
                        0f);
                }

                currentLocation.Map.GetLayer("Buildings")
                    .Draw(mapDisplayDevice, viewport, Location.Origin, false, pixelZoom);
                mapDisplayDevice.EndScene();
                spriteBatch.End();
                spriteBatch.Begin(
                    SpriteSortMode.FrontToBack,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    null,
                    null);
                if (CurrentEvent == null)
                {
                    using (var enumerator3 = currentLocation.characters.GetEnumerator())
                    {
                        while (enumerator3.MoveNext())
                        {
                            var current3 = enumerator3.Current;
                            if (current3 != null && !current3.swimming && !current3.hideShadow
                                && currentLocation.shouldShadowBeDrawnAboveBuildingsLayer(current3.getTileLocation()))
                            {
                                spriteBatch.Draw(
                                    shadowTexture,
                                    GlobalToLocal(
                                        viewport,
                                        current3.position
                                        + new Vector2(
                                            current3.sprite.spriteWidth * pixelZoom / 2f,
                                            current3.GetBoundingBox().Height + (current3.IsMonster ? 0 : pixelZoom * 3))),
                                    shadowTexture.Bounds,
                                    Color.White,
                                    0f,
                                    new Vector2(shadowTexture.Bounds.Center.X, shadowTexture.Bounds.Center.Y),
                                    (pixelZoom + current3.yJumpOffset / 40f) * current3.scale,
                                    SpriteEffects.None,
                                    Math.Max(0f, current3.getStandingY() / 10000f) - 1E-06f);
                            }
                        }

                        goto IL_F5F;
                    }
                }

                foreach (var current4 in CurrentEvent.actors)
                {
                    if (!current4.swimming && !current4.hideShadow
                        && currentLocation.shouldShadowBeDrawnAboveBuildingsLayer(current4.getTileLocation()))
                    {
                        spriteBatch.Draw(
                            shadowTexture,
                            GlobalToLocal(
                                viewport,
                                current4.position
                                + new Vector2(
                                    current4.sprite.spriteWidth * pixelZoom / 2f,
                                    current4.GetBoundingBox().Height + (current4.IsMonster ? 0 : pixelZoom * 3))),
                            shadowTexture.Bounds,
                            Color.White,
                            0f,
                            new Vector2(shadowTexture.Bounds.Center.X, shadowTexture.Bounds.Center.Y),
                            (pixelZoom + current4.yJumpOffset / 40f) * current4.scale,
                            SpriteEffects.None,
                            Math.Max(0f, current4.getStandingY() / 10000f) - 1E-06f);
                    }
                }

                IL_F5F:
                if (!Farmhand.API.Game.Player.swimming && !Farmhand.API.Game.Player.isRidingHorse()
                    && currentLocation.shouldShadowBeDrawnAboveBuildingsLayer(Farmhand.API.Game.Player.getTileLocation()))
                {
                    spriteBatch.Draw(
                        shadowTexture,
                        GlobalToLocal(Farmhand.API.Game.Player.position + new Vector2(32f, 24f)),
                        shadowTexture.Bounds,
                        Color.White,
                        0f,
                        new Vector2(shadowTexture.Bounds.Center.X, shadowTexture.Bounds.Center.Y),
                        4f
                        - ((Farmhand.API.Game.Player.running || Farmhand.API.Game.Player.usingTool) && Farmhand.API.Game.Player.FarmerSprite.indexInCurrentAnimation > 1
                               ? Math.Abs(FarmerRenderer.featureYOffsetPerFrame[Farmhand.API.Game.Player.FarmerSprite.CurrentFrame])
                                 * 0.5f
                               : 0f),
                        SpriteEffects.None,
                        Math.Max(0.0001f, Farmhand.API.Game.Player.getStandingY() / 10000f + 0.00011f) - 0.0001f);
                }

                if (displayFarmer)
                {
                    Farmhand.API.Game.Player.draw(spriteBatch);
                }

                if ((eventUp || killScreen) && !killScreen)
                {
                    currentLocation.currentEvent?.draw(spriteBatch);
                }

                if (Farmhand.API.Game.Player.currentUpgrade != null && Farmhand.API.Game.Player.currentUpgrade.daysLeftTillUpgradeDone <= 3
                    && currentLocation.Name.Equals("Farm"))
                {
                    spriteBatch.Draw(
                        Farmhand.API.Game.Player.currentUpgrade.workerTexture,
                        GlobalToLocal(viewport, Farmhand.API.Game.Player.currentUpgrade.positionOfCarpenter),
                        Farmhand.API.Game.Player.currentUpgrade.getSourceRectangle(),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        (Farmhand.API.Game.Player.currentUpgrade.positionOfCarpenter.Y + tileSize * 3 / 4) / 10000f);
                }

                currentLocation.draw(spriteBatch);
                if (eventUp && currentLocation.currentEvent?.messageToScreen != null)
                {
                    drawWithBorder(
                        currentLocation.currentEvent.messageToScreen,
                        Color.Black,
                        Color.White,
                        new Vector2(
                            Farmhand.API.Game.GraphicsDevice.Viewport.TitleSafeArea.Width / 2
                            - borderFont.MeasureString(currentLocation.currentEvent.messageToScreen).X / 2f,
                            Farmhand.API.Game.GraphicsDevice.Viewport.TitleSafeArea.Height - tileSize),
                        0f,
                        1f,
                        0.999f);
                }

                if (Farmhand.API.Game.Player.ActiveObject == null && (Farmhand.API.Game.Player.UsingTool || pickingTool) && Farmhand.API.Game.Player.CurrentTool != null
                    && (!Farmhand.API.Game.Player.CurrentTool.Name.Equals("Seeds") || pickingTool))
                {
                    drawTool(Farmhand.API.Game.Player);
                }

                if (currentLocation.Name.Equals("Farm"))
                {
                    this.drawFarmBuildings();
                }

                if (tvStation >= 0)
                {
                    spriteBatch.Draw(
                        tvStationTexture,
                        GlobalToLocal(viewport, new Vector2(6 * tileSize + tileSize / 4, 2 * tileSize + tileSize / 2)),
                        new Rectangle(tvStation * 24, 0, 24, 15),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        4f,
                        SpriteEffects.None,
                        1E-08f);
                }

                if (panMode)
                {
                    spriteBatch.Draw(
                        fadeToBlackRect,
                        new Rectangle(
                            (int)Math.Floor((getOldMouseX() + viewport.X) / (double)tileSize) * tileSize - viewport.X,
                            (int)Math.Floor((getOldMouseY() + viewport.Y) / (double)tileSize) * tileSize - viewport.Y,
                            tileSize,
                            tileSize),
                        Color.Lime * 0.75f);
                    foreach (var current5 in currentLocation.warps)
                    {
                        spriteBatch.Draw(
                            fadeToBlackRect,
                            new Rectangle(
                                current5.X * tileSize - viewport.X,
                                current5.Y * tileSize - viewport.Y,
                                tileSize,
                                tileSize),
                            Color.Red * 0.75f);
                    }
                }

                mapDisplayDevice.BeginScene(spriteBatch);
                currentLocation.Map.GetLayer("Front")
                    .Draw(mapDisplayDevice, viewport, Location.Origin, false, pixelZoom);
                mapDisplayDevice.EndScene();
                currentLocation.drawAboveFrontLayer(spriteBatch);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                if (currentLocation.Name.Equals("Farm") && stats.SeedsSown >= 200u)
                {
                    spriteBatch.Draw(
                        debrisSpriteSheet,
                        GlobalToLocal(viewport, new Vector2(3 * tileSize + tileSize / 4, tileSize + tileSize / 3)),
                        getSourceRectForStandardTileSheet(debrisSpriteSheet, 16),
                        Color.White);
                    spriteBatch.Draw(
                        debrisSpriteSheet,
                        GlobalToLocal(viewport, new Vector2(4 * tileSize + tileSize, 2 * tileSize + tileSize)),
                        getSourceRectForStandardTileSheet(debrisSpriteSheet, 16),
                        Color.White);
                    spriteBatch.Draw(
                        debrisSpriteSheet,
                        GlobalToLocal(viewport, new Vector2(5 * tileSize, 2 * tileSize)),
                        getSourceRectForStandardTileSheet(debrisSpriteSheet, 16),
                        Color.White);
                    spriteBatch.Draw(
                        debrisSpriteSheet,
                        GlobalToLocal(viewport, new Vector2(3 * tileSize + tileSize / 2, 3 * tileSize)),
                        getSourceRectForStandardTileSheet(debrisSpriteSheet, 16),
                        Color.White);
                    spriteBatch.Draw(
                        debrisSpriteSheet,
                        GlobalToLocal(viewport, new Vector2(5 * tileSize - tileSize / 4, tileSize)),
                        getSourceRectForStandardTileSheet(debrisSpriteSheet, 16),
                        Color.White);
                    spriteBatch.Draw(
                        debrisSpriteSheet,
                        GlobalToLocal(viewport, new Vector2(4 * tileSize, 3 * tileSize + tileSize / 6)),
                        getSourceRectForStandardTileSheet(debrisSpriteSheet, 16),
                        Color.White);
                    spriteBatch.Draw(
                        debrisSpriteSheet,
                        GlobalToLocal(viewport, new Vector2(4 * tileSize + tileSize / 5, 2 * tileSize + tileSize / 3)),
                        getSourceRectForStandardTileSheet(debrisSpriteSheet, 16),
                        Color.White);
                }

                if (displayFarmer && Farmhand.API.Game.Player.ActiveObject != null && Farmhand.API.Game.Player.ActiveObject.bigCraftable
                    && this.checkBigCraftableBoundariesForFrontLayer()
                    && currentLocation.Map.GetLayer("Front")
                        .PickTile(new Location(Farmhand.API.Game.Player.getStandingX(), Farmhand.API.Game.Player.getStandingY()), viewport.Size) == null)
                {
                    drawPlayerHeldObject(Farmhand.API.Game.Player);
                }
                else if (displayFarmer && Farmhand.API.Game.Player.ActiveObject != null
                         && (currentLocation.Map.GetLayer("Front")
                                 .PickTile(
                                     new Location((int)Farmhand.API.Game.Player.position.X, (int)Farmhand.API.Game.Player.position.Y - tileSize * 3 / 5),
                                     viewport.Size) != null
                             && !currentLocation.Map.GetLayer("Front")
                                 .PickTile(
                                     new Location((int)Farmhand.API.Game.Player.position.X, (int)Farmhand.API.Game.Player.position.Y - tileSize * 3 / 5),
                                     viewport.Size)
                                 .TileIndexProperties.ContainsKey("FrontAlways")
                             || currentLocation.Map.GetLayer("Front")
                                 .PickTile(
                                     new Location(Farmhand.API.Game.Player.GetBoundingBox().Right, (int)Farmhand.API.Game.Player.position.Y - tileSize * 3 / 5),
                                     viewport.Size) != null
                             && !currentLocation.Map.GetLayer("Front")
                                 .PickTile(
                                     new Location(Farmhand.API.Game.Player.GetBoundingBox().Right, (int)Farmhand.API.Game.Player.position.Y - tileSize * 3 / 5),
                                     viewport.Size)
                                 .TileIndexProperties.ContainsKey("FrontAlways")))
                {
                    drawPlayerHeldObject(Farmhand.API.Game.Player);
                }

                if ((Farmhand.API.Game.Player.UsingTool || pickingTool) && Farmhand.API.Game.Player.CurrentTool != null
                    && (!Farmhand.API.Game.Player.CurrentTool.Name.Equals("Seeds") || pickingTool)
                    && currentLocation.Map.GetLayer("Front")
                        .PickTile(
                            new Location(Farmhand.API.Game.Player.getStandingX(), (int)Farmhand.API.Game.Player.position.Y - tileSize * 3 / 5),
                            viewport.Size) != null
                    && currentLocation.Map.GetLayer("Front")
                        .PickTile(new Location(Farmhand.API.Game.Player.getStandingX(), Farmhand.API.Game.Player.getStandingY()), viewport.Size) == null)
                {
                    drawTool(Farmhand.API.Game.Player);
                }

                if (currentLocation.Map.GetLayer("AlwaysFront") != null)
                {
                    mapDisplayDevice.BeginScene(spriteBatch);
                    currentLocation.Map.GetLayer("AlwaysFront")
                        .Draw(mapDisplayDevice, viewport, Location.Origin, false, pixelZoom);
                    mapDisplayDevice.EndScene();
                }

                if (toolHold > 400f && Farmhand.API.Game.Player.CurrentTool.UpgradeLevel >= 1 && Farmhand.API.Game.Player.canReleaseTool)
                {
                    var color = Color.White;
                    switch ((int)(toolHold / 600f) + 2)
                    {
                        case 1:
                            color = Tool.copperColor;
                            break;
                        case 2:
                            color = Tool.steelColor;
                            break;
                        case 3:
                            color = Tool.goldColor;
                            break;
                        case 4:
                            color = Tool.iridiumColor;
                            break;
                    }

                    spriteBatch.Draw(
                        littleEffect,
                        new Rectangle(
                            (int)Farmhand.API.Game.Player.getLocalPosition(viewport).X - 2,
                            (int)Farmhand.API.Game.Player.getLocalPosition(viewport).Y
                            - (Farmhand.API.Game.Player.CurrentTool.Name.Equals("Watering Can") ? 0 : tileSize) - 2,
                            (int)(toolHold % 600f * 0.08f) + 4,
                            tileSize / 8 + 4),
                        Color.Black);
                    spriteBatch.Draw(
                        littleEffect,
                        new Rectangle(
                            (int)Farmhand.API.Game.Player.getLocalPosition(viewport).X,
                            (int)Farmhand.API.Game.Player.getLocalPosition(viewport).Y
                            - (Farmhand.API.Game.Player.CurrentTool.Name.Equals("Watering Can") ? 0 : tileSize),
                            (int)(toolHold % 600f * 0.08f),
                            tileSize / 8),
                        color);
                }

                if (isDebrisWeather && currentLocation.IsOutdoors && !currentLocation.ignoreDebrisWeather
                    && !currentLocation.Name.Equals("Desert") && viewport.X > -10)
                {
                    foreach (var current6 in debrisWeather)
                    {
                        current6.draw(spriteBatch);
                    }
                }

                farmEvent?.draw(spriteBatch);
                if (currentLocation.LightLevel > 0f && timeOfDay < 2000)
                {
                    spriteBatch.Draw(
                        fadeToBlackRect,
                        Farmhand.API.Game.GraphicsDevice.Viewport.Bounds,
                        Color.Black * currentLocation.LightLevel);
                }

                if (screenGlow)
                {
                    spriteBatch.Draw(
                        fadeToBlackRect,
                        Farmhand.API.Game.GraphicsDevice.Viewport.Bounds,
                        screenGlowColor * screenGlowAlpha);
                }

                currentLocation.drawAboveAlwaysFrontLayer(spriteBatch);
                if (Farmhand.API.Game.Player.CurrentTool is FishingRod
                    && ((Farmhand.API.Game.Player.CurrentTool as FishingRod).isTimingCast
                        || (Farmhand.API.Game.Player.CurrentTool as FishingRod).castingChosenCountdown > 0f
                        || (Farmhand.API.Game.Player.CurrentTool as FishingRod).fishCaught
                        || (Farmhand.API.Game.Player.CurrentTool as FishingRod).showingTreasure))
                {
                    Farmhand.API.Game.Player.CurrentTool.draw(spriteBatch);
                }

                if (isRaining && currentLocation.IsOutdoors && !currentLocation.Name.Equals("Desert")
                    && !(currentLocation is Summit)
                    && (!eventUp
                        || currentLocation.isTileOnMap(new Vector2(viewport.X / tileSize, viewport.Y / tileSize))))
                {
                    for (var j = 0; j < rainDrops.Length; j++)
                    {
                        spriteBatch.Draw(
                            rainTexture,
                            rainDrops[j].position,
                            getSourceRectForStandardTileSheet(rainTexture, rainDrops[j].frame),
                            Color.White);
                    }
                }

                spriteBatch.End();

                // base.Draw(gameTime);
                spriteBatch.Begin(
                    SpriteSortMode.FrontToBack,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    null,
                    null);
                if (eventUp && currentLocation.currentEvent != null)
                {
                    foreach (var current7 in currentLocation.currentEvent.actors)
                    {
                        if (current7.isEmoting)
                        {
                            var localPosition = current7.getLocalPosition(viewport);
                            localPosition.Y -= tileSize * 2 + pixelZoom * 3;
                            if (current7.age == 2)
                            {
                                localPosition.Y += tileSize / 2;
                            }
                            else if (current7.gender == 1)
                            {
                                localPosition.Y += tileSize / 6;
                            }

                            spriteBatch.Draw(
                                emoteSpriteSheet,
                                localPosition,
                                new Rectangle(
                                    current7.CurrentEmoteIndex * (tileSize / 4) % emoteSpriteSheet.Width,
                                    current7.CurrentEmoteIndex * (tileSize / 4) / emoteSpriteSheet.Width
                                    * (tileSize / 4),
                                    tileSize / 4,
                                    tileSize / 4),
                                Color.White,
                                0f,
                                Vector2.Zero,
                                4f,
                                SpriteEffects.None,
                                current7.getStandingY() / 10000f);
                        }
                    }
                }

                spriteBatch.End();
                if (drawLighting)
                {
                    spriteBatch.Begin(
                        SpriteSortMode.Deferred,
                        new BlendState
                        {
                            ColorBlendFunction = BlendFunction.ReverseSubtract,
                            ColorDestinationBlend = Blend.One,
                            ColorSourceBlend = Blend.SourceColor
                        },
                        SamplerState.LinearClamp,
                        null,
                        null);
                    spriteBatch.Draw(
                        lightmap,
                        Vector2.Zero,
                        lightmap.Bounds,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        options.lightingQuality,
                        SpriteEffects.None,
                        1f);
                    if (isRaining && currentLocation.isOutdoors && !(currentLocation is Desert))
                    {
                        spriteBatch.Draw(staminaRect, Farmhand.API.Game.GraphicsDevice.Viewport.Bounds, Color.OrangeRed * 0.45f);
                    }

                    spriteBatch.End();
                }

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                if (drawGrid)
                {
                    var num2 = -viewport.X % tileSize;
                    var num3 = -(float)viewport.Y % tileSize;
                    for (var k = num2; k < Farmhand.API.Game.GraphicsDevice.Viewport.Width; k += tileSize)
                    {
                        spriteBatch.Draw(
                            staminaRect,
                            new Rectangle(k, (int)num3, 1, Farmhand.API.Game.GraphicsDevice.Viewport.Height),
                            Color.Red * 0.5f);
                    }

                    for (var num4 = num3;
                         num4 < (float)Farmhand.API.Game.GraphicsDevice.Viewport.Height;
                         num4 += (float)tileSize)
                    {
                        spriteBatch.Draw(
                            staminaRect,
                            new Rectangle(num2, (int)num4, Farmhand.API.Game.GraphicsDevice.Viewport.Width, 1),
                            Color.Red * 0.5f);
                    }
                }

                if (currentBillboard != 0)
                {
                    this.drawBillboard();
                }

                GraphicsEvents.OnPreRenderHudEventNoCheck(this, spriteBatch, gameTime, this.screen);
                if ((displayHUD || eventUp) && currentBillboard == 0 && gameMode == 3 && !freezeControls && !panMode)
                {
                    GraphicsEvents.OnPreRenderHudEvent(this, spriteBatch, gameTime, this.screen);
                    this.drawHUD();
                    GraphicsEvents.OnPostRenderHudEvent(this, spriteBatch, gameTime, this.screen);
                }
                else if (Farmhand.API.Game.ActiveClickableMenu == null && farmEvent == null)
                {
                    spriteBatch.Draw(
                        mouseCursors,
                        new Vector2(getOldMouseX(), getOldMouseY()),
                        getSourceRectForStandardTileSheet(mouseCursors, 0, 16, 16),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        4f + dialogueButtonScale / 150f,
                        SpriteEffects.None,
                        1f);
                }

                GraphicsEvents.OnPostRenderHudEventNoCheck(this, spriteBatch, gameTime, this.screen);

                if (hudMessages.Any() && (!eventUp || isFestival()))
                {
                    for (var l = hudMessages.Count - 1; l >= 0; l--)
                    {
                        hudMessages[l].draw(spriteBatch, l);
                    }
                }
            }

            farmEvent?.draw(spriteBatch);
            if (dialogueUp && !nameSelectUp && !messagePause && !(Farmhand.API.Game.ActiveClickableMenu is DialogueBox))
            {
                this.drawDialogueBox();
            }

            if (progressBar)
            {
                spriteBatch.Draw(
                    fadeToBlackRect,
                    new Rectangle(
                        (Farmhand.API.Game.GraphicsDevice.Viewport.TitleSafeArea.Width - dialogueWidth) / 2,
                        Farmhand.API.Game.GraphicsDevice.Viewport.TitleSafeArea.Bottom - tileSize * 2,
                        dialogueWidth,
                        tileSize / 2),
                    Color.LightGray);
                spriteBatch.Draw(
                    staminaRect,
                    new Rectangle(
                        (Farmhand.API.Game.GraphicsDevice.Viewport.TitleSafeArea.Width - dialogueWidth) / 2,
                        Farmhand.API.Game.GraphicsDevice.Viewport.TitleSafeArea.Bottom - tileSize * 2,
                        (int)(pauseAccumulator / pauseTime * dialogueWidth),
                        tileSize / 2),
                    Color.DimGray);
            }

            if (eventUp)
            {
                currentLocation.currentEvent?.drawAfterMap(spriteBatch);
            }

            if (isRaining && currentLocation.isOutdoors && !(currentLocation is Desert))
            {
                spriteBatch.Draw(staminaRect, Farmhand.API.Game.GraphicsDevice.Viewport.Bounds, Color.Blue * 0.2f);
            }

            if ((fadeToBlack || globalFade) && !menuUp && (!nameSelectUp || messagePause))
            {
                spriteBatch.Draw(
                    fadeToBlackRect,
                    Farmhand.API.Game.GraphicsDevice.Viewport.Bounds,
                    Color.Black * (gameMode == 0 ? 1f - fadeToBlackAlpha : fadeToBlackAlpha));
            }
            else if (flashAlpha > 0f)
            {
                if (options.screenFlash)
                {
                    spriteBatch.Draw(
                        fadeToBlackRect,
                        Farmhand.API.Game.GraphicsDevice.Viewport.Bounds,
                        Color.White * Math.Min(1f, flashAlpha));
                }

                flashAlpha -= 0.1f;
            }

            if ((messagePause || globalFade) && dialogueUp)
            {
                this.drawDialogueBox();
            }

            foreach (var current8 in screenOverlayTempSprites)
            {
                current8.draw(spriteBatch, true);
            }

            if (debugMode)
            {
                spriteBatch.DrawString(
                    smallFont,
                    string.Concat(
                        new object[]
                        {
                            panMode
                                ? (getOldMouseX() + viewport.X) / tileSize + ","
                                  + (getOldMouseY() + viewport.Y) / tileSize
                                : string.Concat(
                                    "aFarmhand.API.Game.Player: ",
                                    Farmhand.API.Game.Player.getStandingX() / tileSize,
                                    ", ",
                                    Farmhand.API.Game.Player.getStandingY() / tileSize),
                            Environment.NewLine, "debugOutput: ", debugOutput
                        }),
                    new Vector2(
                        this.GraphicsDevice.Viewport.TitleSafeArea.X,
                        this.GraphicsDevice.Viewport.TitleSafeArea.Y),
                    Color.Red,
                    0f,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    0.9999999f);
            }

            /*if (inputMode)
                            {
                                spriteBatch.DrawString(smallFont, "Input: " + debugInput, new Vector2(tileSize, tileSize * 3), Color.Purple);
                            }*/
            if (showKeyHelp)
            {
                spriteBatch.DrawString(
                    smallFont,
                    keyHelpString,
                    new Vector2(
                        tileSize,
                        viewport.Height - tileSize
                        - (dialogueUp ? tileSize * 3 + (isQuestion ? questionChoices.Count * tileSize : 0) : 0)
                        - smallFont.MeasureString(keyHelpString).Y),
                    Color.LightGray,
                    0f,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    0.9999999f);
            }

            GraphicsEvents.OnPreRenderGuiEventNoCheck(this, spriteBatch, gameTime, this.screen);
            if (Farmhand.API.Game.ActiveClickableMenu != null)
            {
                GraphicsEvents.OnPreRenderGuiEvent(this, spriteBatch, gameTime, this.screen);
                Farmhand.API.Game.ActiveClickableMenu.draw(spriteBatch);
                GraphicsEvents.OnPostRenderGuiEvent(this, spriteBatch, gameTime, this.screen);
            }
            else
            {
                farmEvent?.drawAboveEverything(spriteBatch);
            }

            GraphicsEvents.OnPostRenderGuiEventNoCheck(this, spriteBatch, gameTime, this.screen);

            GraphicsEvents.OnPostRenderEvent(this, spriteBatch, gameTime, this.screen);
            spriteBatch.End();

            GraphicsEvents.OnDrawInRenderTargetTick(this, spriteBatch, gameTime, this.screen);

            if (!this.ZoomLevelIsOne)
            {
                this.GraphicsDevice.SetRenderTarget(null);
                this.GraphicsDevice.Clear(this.bgColor);
                spriteBatch.Begin(
                    SpriteSortMode.Immediate,
                    BlendState.Opaque,
                    SamplerState.LinearClamp,
                    DepthStencilState.Default,
                    RasterizerState.CullNone);
                spriteBatch.Draw(
                    this.screen,
                    Vector2.Zero,
                    this.screen.Bounds,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    options.zoomLevel,
                    SpriteEffects.None,
                    1f);
                spriteBatch.End();
            }

            GraphicsEvents.OnAfterDraw(this);
        }
    }
}