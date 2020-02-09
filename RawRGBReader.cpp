#include <iostream>
#include <fstream>
using namespace std;

struct RGB
{
	unsigned short R;
	unsigned short G;
	unsigned short B;
};

int main()
{
	char inputFile[] = "C:/Users/masuk/Desktop/gray16bit.tifbin";

	ifstream f(inputFile, ios::in | ios::binary);

	f.seekg(0, ios::end);
	int length = f.tellg();
	f.seekg(0, ios::beg);

	char* buffer = new char[length];
	f.read(buffer, length);

	f.close();

	//RGB* gray16 = (RGB*)(buffer);
	unsigned short* gray16 = (unsigned short*)(buffer);

	const int image_width = 1025;
	const int image_height = 1025;
	
	int pixel_x = 512;
	int pixel_y = 512;

	int pos = ((pixel_y * image_width) + pixel_x);

	//std::cout << (*(gray16 + pos)).R << std::endl;
	std::cout << (*(gray16 + pos)) << std::endl;

	return 0;
}
