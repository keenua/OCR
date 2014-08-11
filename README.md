OCR
===

C# optical character recognition library

A simple example (you can use the [your_image.bmp](your_image.bmp) from the repository)

```C#
// OCR engine
TextRecognition tr = new TextRecognition();

// Create a standard windows font, Tahoma 8px
var font = new StandardFont("Tahoma", 8, FontStyle.Regular);

// Add it to the engine
tr.AddFont(font);

// Load an image
Bitmap image = new Bitmap("your_image.bmp");

// Recognize text
string recognized = tr.Recognize("Tahoma#8", image);

```
