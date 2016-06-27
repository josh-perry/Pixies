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
        public string Name { get; set; }

        /// <summary>
        /// Z-ordering of the layer.
        /// </summary>
        public int ZLevel { get; set; }

        /// <summary>
        /// Image filename.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Full path to file.
        /// </summary>
        public string FullFilePath { get; set; }
    }
}