#include <iostream>
#include <fstream>

// Include unicorn header-file.
#include "unicorn.h"

// Specifications for the data acquisition.
//-------------------------------------------------------------------------------------
#define DATA_FILE				"data.bin"		// The name of the file which is storing acquired data.
#define ACQUISITION_DURATION_S	10.0f			// The acquisition duration in seconds.
#define FRAME_LENGTH			1				// The number of samples acquired per get data call.
#define TESTSIGNAL_ENABLED		true			// Flag to enable or disable testsignal.

// Function declarations.
//-------------------------------------------------------------------------------------
void HandleError(int errorCode);
void PrintErrorMessage(int errorCode);

//-------------------------------------------------------------------------------------
// Main. Program entry point.
//-------------------------------------------------------------------------------------
int main(int argc, char** argv)
{
	std::cout << "Unicorn Acquisition Example" << std::endl;
	std::cout << "---------------------------" << std::endl << std::endl;

	// Variable to store error codes.
	int errorCode = UNICORN_ERROR_SUCCESS;

	// Structure that holds the handle for the currecnt session.
	UNICORN_HANDLE deviceHandle = 0;

	try
	{
		// Get available devices.
		//-------------------------------------------------------------------------------------

		// Get number of available devices.
		unsigned int availableDevicesCount = 0;
		errorCode = UNICORN_GetAvailableDevices(NULL, &availableDevicesCount, TRUE);
		HandleError(errorCode);

		if (availableDevicesCount < 1)
		{
			std::cout << "No device available. Please pair with a Unicorn device first.";
			errorCode = UNICORN_ERROR_GENERAL_ERROR;
		}

		//Get available device serials.
		UNICORN_DEVICE_SERIAL *availableDevices = new UNICORN_DEVICE_SERIAL[availableDevicesCount];
		errorCode = UNICORN_GetAvailableDevices(availableDevices, &availableDevicesCount, true);
		HandleError(errorCode);

		//Print available device serials.
		std::cout << "Available devices:" << std::endl;
		for (unsigned int i = 0; i<availableDevicesCount; i++)
		{
			std::cout << "#" << i << ": " << availableDevices[i] << std::endl;
		}

		// Request device selection.
		std::cout << "\nSelect device by ID #";
		unsigned int deviceSelection;
		std::cin >> deviceSelection;
		if (deviceSelection >= availableDevicesCount || deviceSelection < 0)
			errorCode = UNICORN_ERROR_GENERAL_ERROR;

		HandleError(errorCode);

		// Open selected device.
		//-------------------------------------------------------------------------------------
		std::cout << "Trying to connect to '" << availableDevices[deviceSelection] << "'." << std::endl;
		errorCode = UNICORN_OpenDevice(availableDevices[deviceSelection],&deviceHandle);
		HandleError(errorCode);

		std::cout << "Connected to '" << availableDevices[deviceSelection] << "'." << std::endl;
		std::cout << "Device Handle: " << deviceHandle << std::endl;

		// Create a file to store data.
		std::ofstream file(DATA_FILE, std::ios_base::binary);

		float* acquisitionBuffer = NULL;
		try
		{
			// Initialize acquisition members.
			//-------------------------------------------------------------------------------------
			unsigned int numberOfChannelsToAcquire;
			UNICORN_GetNumberOfAcquiredChannels(deviceHandle, &numberOfChannelsToAcquire);

			UNICORN_AMPLIFIER_CONFIGURATION configuration;
			errorCode = UNICORN_GetConfiguration(deviceHandle, &configuration);
			HandleError(errorCode);

			// Print acquisition configuration
			std::cout << std::endl << "Acquisition Configuration:" << std::endl;
			std::cout << "Frame Length: " << FRAME_LENGTH << std::endl;
			std::cout << "Number Of Acquired Channels: " << numberOfChannelsToAcquire << std::endl;
			std::cout << "Data Acquisition Length: " << ACQUISITION_DURATION_S << "s" << std::endl;

			// Allocate memory for the acquisition buffer.
			int acquisitionBufferLength = numberOfChannelsToAcquire * FRAME_LENGTH;
			acquisitionBuffer = new float[acquisitionBufferLength];

			// Start data acquisition.
			//-------------------------------------------------------------------------------------
			errorCode = UNICORN_StartAcquisition(deviceHandle, TESTSIGNAL_ENABLED);
			HandleError(errorCode);
			std::cout << std::endl << "Data acquisition started." << std::endl;

			// Calculate number of get data calls.
			int numberOfGetDataCalls = (int) (ACQUISITION_DURATION_S * (UNICORN_SAMPLING_RATE / FRAME_LENGTH));

			// Limit console update rate to max. 25Hz or slower.
			int consoleUpdateRate = (int) ((UNICORN_SAMPLING_RATE / FRAME_LENGTH) / 25.0f);
			if (consoleUpdateRate == 0)
				consoleUpdateRate = 1;

			// Acquisition loop.
			//-------------------------------------------------------------------------------------
			for (int i = 0; i < numberOfGetDataCalls; i++)
			{
				// Receives the configured number of samples from the Unicorn device and writes it to the acquisition buffer.
				errorCode = UNICORN_GetData(deviceHandle, FRAME_LENGTH, acquisitionBuffer, acquisitionBufferLength * sizeof(float));
				HandleError(errorCode);

				// Write data to file.
				file.write((const char*) acquisitionBuffer, acquisitionBufferLength * sizeof(float));

				// Update console to indicate that the data acquisition is running.
				if (i%consoleUpdateRate == 0)
					std::cout << ".";
			}

			// Stop data acquisition.
			//-------------------------------------------------------------------------------------
			errorCode = UNICORN_StopAcquisition(deviceHandle);
			HandleError(errorCode);
			std::cout << std::endl << "Data acquisition stopped." << std::endl;
		}
		catch (int errorCode)
		{
			// Write error code to console if something goes wrong.
			PrintErrorMessage(errorCode);
		}
		catch (...)
		{
			// Write error code to console if something goes wrong.
			std::cout << std::endl << "An unknown error occurred." << std::endl;
		}

		// Free memory of the acquisition buffer if necessary.
		if (acquisitionBuffer != NULL)
		{
			delete[] acquisitionBuffer;
			acquisitionBuffer = NULL;
		}

		// Free memory of the device buffer if necessary.
		if (availableDevices != NULL)
		{
			delete[] availableDevices;
			availableDevices = NULL;
		}

		// Close file.
		file.close();

		// Close device.
		//-------------------------------------------------------------------------------------
		errorCode = UNICORN_CloseDevice(&deviceHandle);
		HandleError(errorCode);
		std::cout << "Disconnected from Unicorn." << std::endl;
	}
	catch (int errorCode)
	{
		// Write error code to console if something goes wrong.
		PrintErrorMessage(errorCode);
	}
	catch (...)
	{
		// Write error code to console if something goes wrong.
		std::cout << std::endl << "An unknown error occurred." << std::endl;
	}

	std::cout << std::endl << "Press ENTER to terminate the application.";
	std::cin.clear();
	std::cin.ignore();
	getchar();
	return 0;
}

// The method throws an exception and forwards the error code if something goes wrong.
//-------------------------------------------------------------------------------------
void HandleError(int errorCode)
{
	if (errorCode != UNICORN_ERROR_SUCCESS)
	{
		throw errorCode;
	}
}

// The method prints an error messag to the console according to the error code.
//-------------------------------------------------------------------------------------
void PrintErrorMessage(int errorCode)
{
	std::cout << std::endl << "An error occurred. Error Code: " << errorCode << " - ";
	switch (errorCode)
	{
	case UNICORN_ERROR_INVALID_PARAMETER:
		std::cout << "One of the specified parameters does not contain a valid value.";
		break;
	case UNICORN_ERROR_BLUETOOTH_INIT_FAILED:
		std::cout << "The initialization of the Bluetooth adapter failed.";
		break;
	case UNICORN_ERROR_BLUETOOTH_SOCKET_FAILED:
		std::cout << "The operation could not be performed because the Bluetooth socket failed.";
		break;
	case UNICORN_ERROR_OPEN_DEVICE_FAILED:
		std::cout << "The device could not be opened.";
		break;
	case UNICORN_ERROR_INVALID_CONFIGURATION:
		std::cout << "The configuration is invalid.";
		break;
	case UNICORN_ERROR_BUFFER_OVERFLOW:
		std::cout << "The acquisition buffer is full.";
		break;
	case UNICORN_ERROR_BUFFER_UNDERFLOW:
		std::cout << "The acquisition buffer is empty.";
		break;
	case UNICORN_ERROR_OPERATION_NOT_ALLOWED:
		std::cout << "The operation is not allowed.";
		break;
	case UNICORN_ERROR_INVALID_HANDLE:
		std::cout << "The specified connection handle is invalid.";
		break;
	case UNICORN_ERROR_GENERAL_ERROR:
		std::cout << "An unspecified error occurred.";
		break;
	default:
		break;
	}
	std::cout << std::endl;
}



