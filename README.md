This is a Windows desktop application using WPF / XAML.
Build and run the application in your IDE.

Thid application was a very specifically built project, with a few different modes:
- FILL mode lets you fill out region of the quilt with hexes you manually create with hex codes
- Csv mode pulls in csvs (the ones used are included in the source code) to fill regions
- Mass lets you create hexes using hex codes by esitmating mass of yarn used

The quilt algorithmically calculates regions based on the columns and rows defined in the top left. 
It tries to make regions of ~16 hexes.
