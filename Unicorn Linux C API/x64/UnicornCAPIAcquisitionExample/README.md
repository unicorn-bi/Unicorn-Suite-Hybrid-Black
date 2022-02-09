## Howto Compile

### Compile the example  

`g++ main.cpp -I ../Lib -L ../Lib -lunicorn -o UnicornAcquisitionExample`

or if you run into problems try:

`g++ main.cpp -Wno-narrowing -I ../Lib -L ../Lib -lunicorn -o UnicornAcquisitionExample`

### Add Unicorn C-Lib to LD_LIBRARY PATH
Put this into you .bashrc and replace PATHTOLIBFOLDER with the absolute path to the `../Lib` relative to this directory.  
e.g. /home/youruser/Desktop/Unicorn\ Linux\ C\ API\x64\Lib

`export LD_LIBRARY_PATH=PATHTOLIBFOLDER:$LD_LIBRARY_PATH`

### Execute the Example
`./UnicornAcquisitionExample`
