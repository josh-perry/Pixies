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

        /// <summary>
        /// Z-ordering of the layer.
        /// </summary>
        public int ZLevel
        {
            get; set;
        }
    }
}