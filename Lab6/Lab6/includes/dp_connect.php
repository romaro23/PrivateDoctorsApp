<?php
$conn = new mysqli("localhost", "root", "admin12345", "lab6");
if ($conn->connect_error) {
    die("Помилка підключення: " . $conn->connect_error);
}
$conn->set_charset("utf8");
?>
