using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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
        /// Add a new layer from a file to the current project.
        /// </summary>
        private void AddFileLayer()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";

            if (!openFileDialog.ShowDialog() == true)
                return;

            var filename = Path.GetFileName(openFileDialog.FileName);

            File.Copy(openFileDialog.FileName, Path.Combine(Workspace.Project.FullPath, filename));

            var zLayer = Workspace.Project.Layers.Count;
            var layer = new Layer
            {
                Name = filename,
                ZLevel = zLayer,
                Filename = filename
            };

            Workspace.Project.Layers.Add(layer);
        }

        /// <summary>
        /// Delete the selected layer.
        /// </summary>
        private void DeleteLayer()
        {
            var selectedLayer = (Layer)LstBoxLayers.SelectedValue;

            if(selectedLayer == null)
            {
                return;
            }

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

            return result == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Updates the preview image to show the current layer.
        /// </summary>
        /// <param name="layer"></param>
        private void UpdateImagePreview(Layer layer)
        {
            if (String.IsNullOrEmpty(layer.Filename))
            {
                ImagePreview.Source = null;
                return;
            }

            var uri = Path.Combine(Workspace.Project.FullPath, layer.Filename);

            ImagePreview.Source = new BitmapImage(new Uri(uri));
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
        /// Click handler for new file layer button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewFileLayer_Click(object sender, RoutedEventArgs e)
        {
            AddFileLayer();
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

            if (selectedLayer == null)
                return;

            UpdateImagePreview(selectedLayer);
        }

        /// <summary>
        /// Click handler to export .blend files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportBlend_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new SaveFileDialog();
            openFileDialog.Filter = "Blender files (*.blend) | *.blend;";

            if (!openFileDialog.ShowDialog() == true)
                return;

            var blenderExporter = new Exporters.Blend();
            blenderExporter.Export(Workspace.Project, openFileDialog.FileName);
        }

        private void ExportObj_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new SaveFileDialog();
            openFileDialog.Filter = "Waveform obj files (*.obj) | *.obj;";

            if (!openFileDialog.ShowDialog() == true)
                return;

            var objExporter = new Exporters.ObjExport();
            objExporter.Export(Workspace.Project, openFileDialog.FileName);
        }
        #endregion
    }
}
