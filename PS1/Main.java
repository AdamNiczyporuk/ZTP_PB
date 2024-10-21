import java.util.ArrayList;
import java.util.Scanner;


class Circle {
private double radius;

public Circle(double radius) {
this.radius = radius; }

public double calculatePerimeter() {
return 2 * Math.PI * radius; }

public double calculateArea() {
return Math.PI * radius * radius; } }

class Rectangle {
private double width, height;

public Rectangle(double width, double height) {
this.width = width;
this.height = height; }

public double perimeter() {
return 2 * (width + height); }

public double area() {
return width * height; } }

class Triangle {
private int a, b, c;

public Triangle(int a, int b, int c) {
this.a = a;
this.b = b;
this.c = c; }

public double getPerimeter() {
return a + b + c; }

public double getArea() {
return a * b * c; } }

public class Main {
public static void main(String[] args) {
Scanner scanner = new Scanner(System.in);
ArrayList<Object> figures;

do {
System.out.println("Choose a figure to add (1 - Circle, 2 - Rectangle, 3 - Triangle, 0 - Exit): ");
choice = scanner.nextInt();

if (choice == 1) {
double radius = scanner.nextDouble();
figures.add(new Circle(radius));
} else if (choice == 2) {
double width = scanner.nextDouble()
double height = scanner.nextDouble();
figures.add(new Rectangle(width, height));
} else if (choice == 3) {
double a = scanner.nextDouble();
double b = scanner.nextDouble();
double c = scanner.nextDouble();
figures.add(new Trojkat(a, b, c)); 
} while (choice != 0);

System.out.println("\nFigures in the collection:");
for (Objetc figure : figures) {
if (figure instanceof Circle)
System.out.println("Perimeter: " + ((Circle)figure).calculatePerimeter() + ", Area: " + ((Circle)figure).calculateArea());
else
System.out.println("Perimeter: " + ((Rectangle)figure).perimeter() + ", Area: " + ((Rectangle)figure).area()); }

scanner.close(); } }
