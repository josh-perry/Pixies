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
        
        public MainWindow()
        {
            InitializeComponent();

            Workspace = new Workspace();
            LstBoxLayers.ItemsSource = Workspace.Project.Layers;
            LstBoxLayers.Items.SortDescriptions.Add(new SortDescription("ZLevel", ListSortDirection.Descending));
        }

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
        /// Add an empty layer to the current project.
        /// </summary>
        private void AddBlankLayer()
        {
            var zLayer = Workspace.Project.Layers.Count;
            var layer = new Layer() { Name = "New layer " + zLayer, ZLevel = zLayer };
            Workspace.Project.Layers.Add(layer);
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
    }
}
