<?php
ini_set('display_errors', 1);
ini_set('display_startup_errors', 1);
error_reporting(E_ALL);
include '../includes/dp_connect.php';
$conn->set_charset("utf8mb4");
$serviceName = $price = $duration = "";
$isEdit = false;
if (isset($_GET['id'])) {
    $id = intval($_GET['id']);
    $isEdit = true;
    $stmt = $conn->prepare("SELECT * FROM Services WHERE ID = ?");
    $stmt->bind_param("i", $id);
    $stmt->execute();
    $res = $stmt->get_result();
    if ($res->num_rows === 1) {
        $row = $res->fetch_assoc();
        $serviceName = $row['ServiceName'];
        $price = $row['Price'];
        $duration = $row['DurationMinutes'];
    }
    $stmt->close();
}
if (isset($_POST['submit'])) {
    $serviceName = $_POST['ServiceName'];
    $price = $_POST['Price'];
    $duration = $_POST['Duration'];

    if (!empty($_POST['id'])) {
        $id = intval($_POST['id']);
        $stmt = $conn->prepare("UPDATE Services SET ServiceName=?, Price=?, DurationMinutes=? WHERE ID=?");
        $stmt->bind_param("sssi", $serviceName, $price, $duration, $id);
        $stmt->execute();
        $stmt->close();
        header("Location: services.php");
        exit;
    } else {
        $stmt = $conn->prepare("INSERT INTO Services (ServiceName, Price, DurationMinutes) VALUES (?, ?, ?)");
        $stmt->bind_param("sss", $serviceName, $price, $duration);
        $stmt->execute();
        $stmt->close();
        header("Location: services.php");
        exit;
    }
}
?>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <title>Employee</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 40px;
            background-color: #f9f9f9;
            color: #333;
        }
        h1, h2 {
            color: #2c3e50;
        }
        form {
            background-color: #ffffff;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0,0,0,0.1);
            width: 300px;
            margin-bottom: 30px;
        }
        label {
            font-size: 14px;
            display: block;
            margin-bottom: 4px;
            margin-top: 10px;
        }
        input[type="text"],
        input[type="email"] {
            width: 100%;
            padding: 4px 8px;
            font-size: 14px;
            border: 1px solid #ccc;
            border-radius: 4px;
            box-sizing: border-box;
        }
        input[type="submit"],
        .btn {
            background-color: #3498db;
            color: white;
            padding: 6px 12px;
            font-size: 14px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            text-decoration: none;
        }
        input[type="submit"]:hover,
        .btn:hover {
            background-color: #2980b9;
        }
        table {
            width: 100%;
            border-collapse: collapse;
            background-color: #fff;
            box-shadow: 0 0 10px rgba(0,0,0,0.05);
        }
        th, td {
            padding: 8px 10px;
            border: 1px solid #ddd;
            text-align: left;
        }
        th {
            background-color: #f0f0f0;
        }
        tr:nth-child(even) {
            background-color: #fafafa;
        }
        .submit-container {
            display: flex;
            justify-content: center;
            margin-top: 16px;
        }

    </style>
</head>
<body>

<h1><?= $isEdit ? 'Change service' : 'Add service' ?></h1>
<form action="services.php" method="post">
    <input type="hidden" name="id" value="<?= htmlspecialchars($_GET['id'] ?? '') ?>">
    <label>Service name:</label>
    <input type="text" name="ServiceName" required value="<?= htmlspecialchars($serviceName) ?>">
    
    <label>Price:</label>
    <input type="text" name="Price" required value="<?= htmlspecialchars($price) ?>">
    
    <label>Duration:</label>
    <input type="text" name="Duration" value="<?= htmlspecialchars($duration) ?>">

    <div class="submit-container">
        <input type="submit" name="submit" value="<?= $isEdit ? 'Change' : 'Add' ?>">
    </div>
</form>

<?php

$result = $conn->query("SELECT * FROM Services");
if ($result->num_rows > 0) {
    echo "<h2>Services List</h2>";
    echo "<table>";
    echo "<tr>
            <th>ID</th>
            <th>Service Name</th>
            <th>Price</th>
            <th>Duration</th>
        </tr>";
    while ($row = $result->fetch_assoc()) {
        $id = htmlspecialchars($row['ID']);
        echo "<tr>
            <td>$id</td>
            <td>" . htmlspecialchars($row['ServiceName']) . "</td>
            <td>" . htmlspecialchars($row['Price']) . "</td>
            <td>" . htmlspecialchars($row['DurationMinutes']) . "</td>
            <td><a class='btn' href='services.php?id=" . $row['ID'] . "'>Change</a></td>
            <td><a class='btn' style='background-color:#e74c3c;' href='../scripts/delete_service.php?id=$id' onclick=\"return confirm('Are you sure?');\">Delete</a></td>
        </tr>";
    }
    echo "</table>";
} else {
    echo "<p>No services to show.</p>";
}
?>

</body>
</html>
