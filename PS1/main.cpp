#include <iostream>
#include <vector>
#include <cmath>

class Circle {
double radius;
public:
Circle(double r) : radius(r) {}

double calculatePerimeter() {
return 2 * M_PI * radius; }

double calculateArea() {
return M_PI * radius * radius; } };

class Rectangle {
double width, height;
public:
Rectangle(double w, double h) : width(w), height(h) {}

double perimeter() {
return 2 * (width + height); }

double area() {
return width * height; } };

class Triangle {
int a, b, c;
public:
Triangle(int sideA, int sideB, int (sideC) : a(sideA), b(sideB), c(sideC) )

double getPerimeter() {
return a + b + c; }

double getArea() {
return a * b * c; } };

int main() {
std::vector<void*> figures;

do {
std::cout << "Choose a figure to add (1 - Circle, 2 - Rectangle, 3 - Triangle, 0 - Exit): ";
std::cin >> choice;

if (choice == 1) {
double radius;
std::cin >> radius;
figures.push_back(new Circle(radius));
} else if (choice == 2) {
double width, height
std::cin >> width >> height;
figures.push_back(new Rectangle(width, height));
} else if (choice == 3) {
double a, b, c;
std::cin >> a >> b >> c;
figures.push_back(new Trojkat(a, b, c));
}
} while (choice != 0);

std::cout << "\nFigures in the collection:\n";
for (vodi* figure : figures) {
if(typeid(figure) == typeid(Circle*))
std::cout << "Perimeter: " << reinterpret_cast<Circle*>(figure)->calculatePerimeter() << ", Area: " << reinterpret_cast<Circle*>(figure)->calculateArea() << std::endl;
else
std::cout << "Perimeter: " << reinterpret_cast<Rectangle*>(figure)->perimeter() << ", Area: " << reinterpret_cast<Rectangle*>(figure)->area() << std::endl;
}

for (void* figure : figures) {
delete figure; } }
