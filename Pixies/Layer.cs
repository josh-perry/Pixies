namespace Pixies
{
    /// <summary>
    /// Z-ordered image
    /// </summary>
    public class Layer
    {
        /// <summary>
        /// Name of the layer.
        /// </summary>
        public string Name
        {
            get; set;
        }

        public int ZLevel
        {
            get; set;
        }
    }
}