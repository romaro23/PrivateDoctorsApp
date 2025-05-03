<?php
include '../includes/dp_connect.php';
$conn->set_charset("utf8mb4");

if (isset($_GET['id'])) {
    $id = intval($_GET['id']);

    $stmt = $conn->prepare("DELETE FROM Patients WHERE ID = ?");
    $stmt->bind_param("i", $id);

    if ($stmt->execute()) {
        header("Location: ../pages/patients.php");

        exit();
    } else {
        echo "Error deleting record: " . htmlspecialchars($conn->error);
    }

    $stmt->close();
} else {
    echo "No ID provided.";
}
?>
