1. compile the example (change PATHTOLIBFOLDER to your individual folderpath e.g. /home/pi/Desktop/Lib)
g++ main.cpp -I PATHTOLIBFOLDER -L PATHTOLIBFOLDER -lunicorn -o UnicornAcquisitionExample

2. add library to environment variable
export LD_LIBRARY_PATH=PATHTOLIBFOLDER:$LD_LIBRARY_PATH

3. execute the exemple
./UnicornAcquisitionExample