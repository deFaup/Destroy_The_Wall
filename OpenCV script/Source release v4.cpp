#include "opencv2/core.hpp"
#include "opencv2/imgproc.hpp" // For cvtColor, rectangle, putText, resize
#include "opencv2/highgui.hpp" // For VideoCapture, CAP_PROP_FRAME_WIDTH/HEIGHT, imshow, waitKey, destroyAllWindows, moveWindow

#include <iostream>
#include <chrono>
#include <thread>

using namespace cv;
using namespace std;
using namespace std::this_thread;     // sleep_for, sleep_until
using namespace std::chrono_literals; // ns, us, ms, s, h, etc.
using std::chrono::system_clock;

// Global variable

String _window = "Brick Game with Unity & OpenCV";
VideoCapture _capture;
//x axis _outCameraWidth
int _x;
//y axis _outCameraHeight
int _y;
Mat cadre_h_g_init, cadre_h_init, cadre_h_d_init;
Mat flip_h_g_init, flip_h_init, flip_h_d_init;

int seuil = 40;
cv::Rect _h_g;
cv::Rect _h;
cv::Rect _h_d;

extern "C" int __declspec(dllexport) __stdcall Init() {

	// destroy previous window if we relaunch the programm in order to put this new window on top of the others
	//destroyWindow(_window);

	// First : we open the webcam stream and set up the camera in HD  mode 1280x720
	_capture.open(0);
	if (!_capture.isOpened())
		return -1;

	// Turn off autofocus (1) is on, not always working, depends on the hardware
	_capture.set(CAP_PROP_AUTOFOCUS, 0);

	// cv::CAP_PROP_FRAME_WIDTH =3,	cv::CAP_PROP_FRAME_HEIGHT = 4 Better in HD
	//_capture.set(3, 1280);
	//_capture.set(4, 720);

	// Two : get image dimensions and set up frames
	_x = _capture.get(CAP_PROP_FRAME_WIDTH);
	_y = _capture.get(CAP_PROP_FRAME_HEIGHT);

	_h_g = Rect(0, 0, int(_x / 3), int(_y / 3));
	_h = Rect(int(_x / 3), 0, int(_x / 3), _y / 3);
	_h_d = Rect(2 * int(_x / 3), 0, int(_x / 3), _y / 3);

	//Three : we save up and up left/right images default matrices
	Mat image;
	Mat gray_image;
	Mat flipped;
	Mat equ_hg;
	Mat equ_h;
	Mat equ_hd;

	sleep_until(system_clock::now() + 1s);

	_capture >> image;
	flip(image, flipped, 1);	// If using frontal camera images need to be flipped
	cvtColor(flipped, gray_image, CV_BGR2GRAY);

	equalizeHist(gray_image(_h_g), cadre_h_g_init);
	equalizeHist(gray_image(_h), cadre_h_init);
	equalizeHist(gray_image(_h_d), cadre_h_d_init);

	return 0;
}

int mse(Mat imageA, Mat imageB) {
	//int mse = s / (imageA.channels() * imageA.total());
	Scalar s = sum((imageA - imageB) * 2);
	return (int(s.val[0] + s.val[1] + s.val[2]) / (imageA.rows * imageA.cols));
}

extern "C" void __declspec(dllexport) __stdcall Close()
{
	_capture.release();
	// When everything done, release the capture
	destroyWindow(_window);
}

int draw(Mat cadre, Mat cadre_init, Mat flipped, Point p, int i, int seuil) {
	/**
	1. Reckon the mse; 2. Test the distance between init and actual frame
	3. Draw rectangles 4. save the text " MSE = xx " in var str;
	5. put text on provided image **/
	char str[20];
	int value_of_mse = mse(cadre, cadre_init);

	if (value_of_mse > seuil && i == 1) {
		rectangle(flipped, _h_g, cv::Scalar(0, 0, 255)); //red
	}
	else if (value_of_mse > seuil && i == 2) {
		rectangle(flipped, _h, cv::Scalar(0, 0, 255));
	}
	else if (value_of_mse > seuil && i == 3) {
		rectangle(flipped, _h_d, cv::Scalar(0, 0, 255));
	}
	else {
		if (i == 1)
			rectangle(flipped, _h_g, cv::Scalar(0, 255, 0));
		if (i == 2)
			rectangle(flipped, _h, cv::Scalar(0, 255, 0));
		if (i == 3)
			rectangle(flipped, _h_d, cv::Scalar(0, 255, 0));
	}

	sprintf(str, " MSE = %d", value_of_mse);
	putText(flipped, str, p, FONT_HERSHEY_SIMPLEX, 1, cv::Scalar(255, 255, 255));

	return value_of_mse;
}

extern "C" void __declspec(dllexport) __stdcall main(int& mse_h_g, int& mse_h, int& mse_h_d, int& seuil, bool& down) {

	Mat image;
	Mat gray_image;
	Mat flipped;
	Mat equ_hg;
	Mat equ_h;
	Mat equ_hd;

	_capture >> image;
	flip(image, flipped, 1);
	//rotate the image as the image from frontal camera are inversed

		cvtColor(flipped, gray_image, CV_BGR2GRAY);
		equalizeHist(gray_image(_h_g), equ_hg);
		equalizeHist(gray_image(_h), equ_h);
		equalizeHist(gray_image(_h_d), equ_hd);

		if (down) {
			equalizeHist(gray_image(_h_g), cadre_h_g_init);
			equalizeHist(gray_image(_h),   cadre_h_init);
			equalizeHist(gray_image(_h_d), cadre_h_d_init);
		}

		// Draw rectangles on colored pic, get the mse of this rectangles (compared with inital ones) and show the result in the flipped image

		mse_h_g = draw(equ_hg, cadre_h_g_init, flipped, Point(0, int(_y / 3) - 10), 1, seuil);
		mse_h = draw(equ_h, cadre_h_init, flipped, Point(int(_x / 3), int(_y / 3) - 10), 2, seuil);
		mse_h_d = draw(equ_hd, cadre_h_d_init, flipped, Point(int(2 * _x / 3), int(_y / 3) - 10), 3, seuil);

		// Picutres shown are resized to 853x480 853 = (480/720)*1280
		
		Mat dst;
		resize(flipped, dst, Size(int(_x * 400 / _y), 400), INTER_AREA);
		
		int key = waitKey(1);

		imshow(_window, dst);
		moveWindow(_window, 0, 550);

		_capture >> image;
		flip(image, flipped, 1);

}
