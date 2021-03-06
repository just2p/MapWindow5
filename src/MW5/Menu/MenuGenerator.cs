﻿using System;
using System.Collections.Generic;
using MW5.Plugins;
using MW5.Plugins.Concrete;
using MW5.Plugins.Interfaces;
using MW5.Properties;
using MW5.Services;
using System.Linq;
using MW5.Plugins.Enums;
using MW5.Plugins.Services;
using MW5.Services.Helpers;
using MW5.Shared;
using MW5.UI.Menu;
using ViewMenuHelper = MW5.UI.Helpers.ViewMenuHelper;

namespace MW5.Menu
{
    internal class MenuGenerator
    {
        private const string FileToolbarNane = "File";
        private const string MapToolbarName = "Map";
        
        private readonly IAppContext _context;
        private readonly IPluginManager _pluginManager;
        private readonly MenuCommands _commands;
        private readonly object _menuManager;
        private readonly object _dockingManager;

        public MenuGenerator(IAppContext context, IPluginManager pluginManager, IMainView mainView)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (pluginManager == null) throw new ArgumentNullException("pluginManager");
            if (mainView == null) throw new ArgumentNullException("mainView");

            _context = context;
            _pluginManager = pluginManager;
            _menuManager = mainView.MenuManager;
            _dockingManager = mainView.DockingManager;
            _commands = new MenuCommands(PluginIdentity.Default);

            InitToolbars();

            InitMenus();
        }

        private void InitMenus()
        {
            InitFileMenu();

            InitLayerMenu();

            InitMapMenu();

            InitViewMenu();

            InitPluginsMenu();

            InitTilesMenu();

            InitToolsMenu();

            InitHelpMenu();

            var items = _context.Menu.Items;
            items.InsertBefore = items[items.Count - 1];

            _context.Menu.Update();
        }

        #region Menus

        private void InitToolsMenu()
        {
            var items = _context.Menu.ToolsMenu.SubItems;

            items.AddButton(_commands[MenuKeys.ShowRepository]);
            items.AddButton(_commands[MenuKeys.ShowToolbox]);

            var item = items.AddButton(_commands[MenuKeys.Settings], true);

            items.InsertBefore = item;

            _context.Menu.ToolsMenu.Update();
        }

        private void InitFileMenu()
        {
            var items = _context.Menu.FileMenu.SubItems;

            items.AddButton(_commands[MenuKeys.NewMap]);
            
            items.AddButton(_commands[MenuKeys.AddLayer], true);
            items.AddButton(_commands[MenuKeys.AddVectorLayer], true);
            items.AddButton(_commands[MenuKeys.AddRasterLayer]);
            items.AddButton(_commands[MenuKeys.AddDatabaseLayer]);
            items.AddButton(_commands[MenuKeys.AddWmsLayer]);

            items.AddButton(_commands[MenuKeys.OpenProject], true);
            items.AddButton(_commands[MenuKeys.SaveProject], true);
            items.AddButton(_commands[MenuKeys.SaveProjectAs]);

            items.AddButton(_commands[Plugins.Menu.MenuKeys.Quit], true);

            _context.Menu.FileMenu.Update();
        }

        private void InitMapMenu()
        {
            var items = _context.Menu.MapMenu.SubItems;

            items.AddButton(_commands[MenuKeys.FindLocation]);

            items.AddButton(_commands[MenuKeys.SetScale], true);
            items.AddButton(_commands[MenuKeys.SetProjection]);

            items.AddButton(_commands[MenuKeys.RemoveLayer], true);
            items.AddButton(_commands[MenuKeys.ClearLayers]);

            items.AddButton(_commands[MenuKeys.ClearSelection], true);

            _context.Menu.MapMenu.Update();
        }

        private void InitLayerMenu()
        {
            var items = _context.Menu.LayerMenu.SubItems;
            
            items.AddButton(_commands[MenuKeys.LayerClearSelection]);

            _context.Menu.LayerMenu.Update();
        }

        private void InitPluginsMenu()
        {
            var menu = _context.Menu.PluginsMenu;
            menu.SubItems.AddButton("Configure Plugins...", MenuKeys.PluginsConfigure, PluginIdentity.Default).BeginGroup = true;
            
            PluginsMenuHelper.Init(_context, _pluginManager);
        }

        private void InitTilesMenu()
        {
            var menu = _context.Menu.TilesMenu;

            menu.SubItems.AddButton("Set Bing API key", MenuKeys.BingApiKey, PluginIdentity.Default).BeginGroup = true;
            menu.SubItems.AddButton("Configure Tiles...", MenuKeys.TilesConfigure, PluginIdentity.Default).BeginGroup = true;
            
            TilesMenuHelper.Init(_context, _context.Menu.TilesMenu);
        }

        private void InitViewMenu()
        {
            var items = _context.Menu.ViewMenu.SubItems;

            items.AddButton(_commands[MenuKeys.ZoomIn]);
            items.AddButton(_commands[MenuKeys.ZoomOut]);
            items.AddButton(_commands[MenuKeys.ZoomMax]);
            items.AddButton(_commands[MenuKeys.ZoomPrev], true);
            items.AddButton(_commands[MenuKeys.ZoomNext]);
            items.AddButton(_commands[MenuKeys.ZoomToLayer], true);
            items.AddButton(_commands[MenuKeys.ZoomToBaseLayer]);

            items.AddButton(_commands[MenuKeys.ShowZoombar], true);
            items.AddButton(_commands[MenuKeys.ShowScalebar]);
            items.AddButton(_commands[MenuKeys.ShowCoordinates]);
            items.AddButton(_commands[MenuKeys.ShowRedrawTime]);

            items.AddDropDown("Toolbars", Plugins.Menu.MenuKeys.ViewToolbars, PluginIdentity.Default).BeginGroup = true;
            items.AddDropDown("Windows", Plugins.Menu.MenuKeys.ViewWindows, PluginIdentity.Default);

            _context.Menu.ViewMenu.Update();

            ViewMenuHelper.Init(_menuManager, _dockingManager, _context.Menu, _context.DockPanels, PluginIdentity.Default);
        }

        private void InitHelpMenu()
        {
            var items = _context.Menu.HelpMenu.SubItems;
            items.AddButton("Show Welcome Screen", MenuKeys.Welcome, PluginIdentity.Default);
            items.AddButton("Supported Drivers", MenuKeys.SupportedDrivers, PluginIdentity.Default);
            items.AddButton("COM Usage", MenuKeys.ComUsage, PluginIdentity.Default).BeginGroup = true;
            items.AddButton("About", MenuKeys.About, PluginIdentity.Default).BeginGroup = true;

            _context.Menu.HelpMenu.Update();
        }

        #endregion

        #region Toolbars

        private void InitToolbars()
        {
            var bar = _context.Toolbars.Add(FileToolbarNane, ToolbarCollectionBase.FILE_TOOLBAR_KEY, PluginIdentity.Default);
            InitFileToolbar(bar);
            bar.DockState = ToolbarDockState.Left;

            bar = _context.Toolbars.Add(MapToolbarName, ToolbarCollectionBase.MAP_TOOLBAR_KEY, PluginIdentity.Default);
            InitMapToolbar(bar);
            bar.DockState = ToolbarDockState.Top;
        }

        private void InitFileToolbar(IToolbar bar)
        {
            var items = bar.Items;

            items.AddButton(_commands[MenuKeys.NewMap]);
            items.AddButton(_commands[MenuKeys.OpenProject]);
            items.AddButton(_commands[MenuKeys.SaveProject]);
            items.AddButton(_commands[MenuKeys.SaveProjectAs]);
            items.AddButton(_commands[MenuKeys.AddLayer], true);
            items.AddButton(_commands[MenuKeys.AddVectorLayer]);
            items.AddButton(_commands[MenuKeys.AddRasterLayer]);
            items.AddButton(_commands[MenuKeys.AddDatabaseLayer]);
            items.AddButton(_commands[MenuKeys.AddWmsLayer]);
            items.AddButton(_commands[MenuKeys.RemoveLayer]);
            items.AddButton(_commands[MenuKeys.Settings], true);
            
            #if DEBUG

            if (DebugHelper.ShowDebugMenuElements)
            {
                items.AddButton(_commands[MenuKeys.Test], true);    
            }

            #endif
            
            items.InsertBefore = items[items.Count - 2];    // before settings

            bar.Update();
        }

        private void InitMapToolbar(IToolbar bar)
        {
            var items = bar.Items;

            items.AddButton(_commands[MenuKeys.ZoomIn]);
            items.AddButton(_commands[MenuKeys.ZoomOut]);
            items.AddButton(_commands[MenuKeys.ZoomMax]);
            items.AddButton(_commands[MenuKeys.ZoomPrev]);
            items.AddButton(_commands[MenuKeys.ZoomNext]);
            items.AddButton(_commands[MenuKeys.ZoomToLayer]);
            items.AddButton(_commands[MenuKeys.Pan]);
            items.AddButton(_commands[MenuKeys.SetProjection], true);
            items.AddButton(_commands[MenuKeys.FindLocation]);

            items.AddButton(_commands[MenuKeys.MeasureDistance], true);
            items.AddButton(_commands[MenuKeys.MeasureArea]);

            // select drop down
            var dropDown = items.AddDropDown("Select", MenuKeys.SelectDropDown, PluginIdentity.Default);
            dropDown.BeginGroup = true;
            dropDown.Icon = new MenuIcon(Resources.icon_select);

            dropDown.SubItems.AddButton(_commands[MenuKeys.SelectByRectangle]);
            dropDown.SubItems.AddButton(_commands[MenuKeys.SelectByPolygon]);

            items.AddButton(_commands[MenuKeys.ZoomToSelected]);
            items.AddButton(_commands[MenuKeys.ClearSelection]);

            bar.Update();
        }

        #endregion
    }
}
