using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Pixies
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Workspace Workspace;

        #region Constructors
        /// <summary>
        /// Constructor. Set up binding and sorting properties.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            Workspace = new Workspace();
            LstBoxLayers.ItemsSource = Workspace.Project.Layers;
            LstBoxLayers.Items.SortDescriptions.Add(new SortDescription("ZLevel", ListSortDirection.Descending));
        }
        #endregion

        /// <summary>
        /// Add an empty layer to the current project.
        /// </summary>
        private void AddBlankLayer()
        {
            var zLayer = Workspace.Project.Layers.Count;
            var layer = new Layer() { Name = "New layer " + zLayer, ZLevel = zLayer };
            Workspace.Project.Layers.Add(layer);
        }

        /// <summary>
        /// Delete the selected layer.
        /// </summary>
        private void DeleteLayer()
        {
            Layer selectedLayer = (Layer)LstBoxLayers.SelectedValue;

            if (!DeleteLayerConfirmation(selectedLayer))
            {
                return;
            }

            Workspace.Project.Layers.Remove(selectedLayer);
        }

        /// <summary>
        /// Show dialog box asking to confirm deletion
        /// </summary>
        /// <param name="layer">Layer to delete</param>
        /// <returns>true if we should delete it, false otherwise.</returns>
        private bool DeleteLayerConfirmation(Layer layer)
        {
            if(Properties.Settings.Default.NeverAskForDeleteConfirmation)
            {
                return true;
            }

            var message = $"Are you sure you want to delete layer '{layer.Name}'?";
            var title = "Pixies";

            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Information);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    return true;
                case MessageBoxResult.No:
                case MessageBoxResult.Cancel:
                default:
                    return false;
            }
        }

        #region UI Event Handlers
        /// <summary>
        /// Click handler for new blank layer button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewBlankLayer_Click(object sender, RoutedEventArgs e)
        {
            AddBlankLayer();
        }

        /// <summary>
        /// Click handler for delete selected layer button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteLayer_Click(object sender, RoutedEventArgs e)
        {
            DeleteLayer();
        }

        /// <summary>
        /// Selection changed event on layer list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LstBoxLayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listbox = (ListBox) sender;
            Layer selectedLayer = (Layer) listbox.SelectedValue;
        }
        #endregion
    }
}
