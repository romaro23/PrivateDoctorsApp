<?php
include '../includes/dp_connect.php';
$conn->set_charset("utf8mb4");
$lastName = $firstName = $middleName = $contactNumber = $email = $address = "";
$isEdit = false;

if (isset($_GET['id'])) {
    $id = intval($_GET['id']);
    $isEdit = true;
    $stmt = $conn->prepare("SELECT * FROM Employee WHERE ID = ?");
    $stmt->bind_param("i", $id);
    $stmt->execute();
    $res = $stmt->get_result();
    if ($res->num_rows === 1) {
        $row = $res->fetch_assoc();
        $lastName = $row['LastName'];
        $firstName = $row['FirstName'];
        $middleName = $row['MiddleName'];
        $contactNumber = $row['ContactNumber'];
        $email = $row['Email'];
        $address = $row['Address'];
    }
    $stmt->close();
}

if (isset($_POST['submit'])) {
    $lastName = $_POST['LastName'];
    $firstName = $_POST['FirstName'];
    $middleName = $_POST['MiddleName'];
    $contactNumber = $_POST['ContactNumber'];
    $email = $_POST['Email'];
    $address = $_POST['Address'];

    if (!empty($_POST['id'])) {
        $id = intval($_POST['id']);
        $stmt = $conn->prepare("UPDATE Employee SET LastName=?, FirstName=?, MiddleName=?, ContactNumber=?, Email=?, Address=? WHERE ID=?");
        $stmt->bind_param("ssssssi", $lastName, $firstName, $middleName, $contactNumber, $email, $address, $id);
        $stmt->execute();
        $stmt->close();
        header("Location: employee.php");
        exit;
    } else {
        $stmt = $conn->prepare("INSERT INTO Employee (LastName, FirstName, MiddleName, ContactNumber, Email, Address) VALUES (?, ?, ?, ?, ?, ?)");
        $stmt->bind_param("ssssss", $lastName, $firstName, $middleName, $contactNumber, $email, $address);
        $stmt->execute();
        $stmt->close();
        header("Location: employee.php");
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

<h1><?= $isEdit ? 'Change employee' : 'Add employee' ?></h1>
<form action="employee.php" method="post">
    <input type="hidden" name="id" value="<?= htmlspecialchars($_GET['id'] ?? '') ?>">
    
    <label>Last name:</label>
    <input type="text" name="LastName" required value="<?= htmlspecialchars($lastName) ?>">
    
    <label>First name:</label>
    <input type="text" name="FirstName" required value="<?= htmlspecialchars($firstName) ?>">
    
    <label>Middle name:</label>
    <input type="text" name="MiddleName" value="<?= htmlspecialchars($middleName) ?>">
    
    <label>Contact number:</label>
    <input type="text" name="ContactNumber" value="<?= htmlspecialchars($contactNumber) ?>">
    
    <label>Email:</label>
    <input type="email" name="Email" value="<?= htmlspecialchars($email) ?>">
    
    <label>Address:</label>
    <input type="text" name="Address" value="<?= htmlspecialchars($address) ?>">

    <div class="submit-container">
        <input type="submit" name="submit" value="<?= $isEdit ? 'Change' : 'Add' ?>">
    </div>
</form>


<?php
$result = $conn->query("SELECT * FROM Employee");
if ($result->num_rows > 0) {
    echo "<h2>Employee List</h2>";
    echo "<table>";
    echo "<tr>
            <th>ID</th>
            <th>Last Name</th>
            <th>First Name</th>
            <th>Middle Name</th>
            <th>Phone</th>
            <th>Email</th>
            <th>Address</th>
            <th>Change</th>
            <th>Delete</th>
        </tr>";
    while ($row = $result->fetch_assoc()) {
        $id = htmlspecialchars($row['ID']);
        echo "<tr>
            <td>$id</td>
            <td>" . htmlspecialchars($row['LastName']) . "</td>
            <td>" . htmlspecialchars($row['FirstName']) . "</td>
            <td>" . htmlspecialchars($row['MiddleName']) . "</td>
            <td>" . htmlspecialchars($row['ContactNumber']) . "</td>
            <td>" . htmlspecialchars($row['Email']) . "</td>
            <td>" . htmlspecialchars($row['Address']) . "</td>
            <td><a class='btn' href='employee.php?id=" . $row['ID'] . "'>Change</a></td>
            <td><a class='btn' style='background-color:#e74c3c;' href='../scripts/delete_employee.php?id=$id' onclick=\"return confirm('Are you sure?');\">Delete</a></td>
        </tr>";
    }
    echo "</table>";
} else {
    echo "<p>No employees to show.</p>";
}
?>

</body>
</html>
