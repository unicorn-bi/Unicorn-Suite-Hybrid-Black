## Howto Compile

### Compile the example  

`g++ main.cpp -I ../Lib -L ../Lib -lunicorn -o UnicornAcquisitionExample`

### Add Unicorn C-Lib to LD_LIBRARY PATH
Put this into you .bashrc  
`export LD_LIBRARY_PATH=PATHTOLIBFOLDER:$LD_LIBRARY_PATH`

### Execute the Example
`./UnicornAcquisitionExample`
