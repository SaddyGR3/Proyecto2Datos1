param (
    [Parameter(Mandatory = $true)]
    [string]$IP,
    [Parameter(Mandatory = $true)]
    [int]$Port
)

$ipEndPoint = [System.Net.IPEndPoint]::new([System.Net.IPAddress]::Parse("127.0.0.1"), 11000)

function Send-Message {
    param (
        [Parameter(Mandatory=$true)]
        [pscustomobject]$message,
        [Parameter(Mandatory=$true)]
        [System.Net.Sockets.Socket]$client
    )

    $stream = New-Object System.Net.Sockets.NetworkStream($client)
    $writer = New-Object System.IO.StreamWriter($stream)
    try {
        $writer.WriteLine($message)
    }
    finally {
        $writer.Close()
        $stream.Close()
    }
}


function Send-SQLCommand {
    param (
        [string]$command
    )
    $client = New-Object System.Net.Sockets.Socket($ipEndPoint.AddressFamily, [System.Net.Sockets.SocketType]::Stream, [System.Net.Sockets.ProtocolType]::Tcp)
    $client.Connect($ipEndPoint)

    $requestType = if ($command -match "^INSERT") { 1 }
    elseif ($command -match "^DELETE") { 2 }
    elseif ($command -match "^SELECT") { 3 }
    else { 0 }

    $requestObject = [PSCustomObject]@{
        RequestType = $requestType;
        RequestBody = $command
    }

    Write-Host -ForegroundColor Green "Sending command: $command"

    $jsonMessage = ConvertTo-Json -InputObject $requestObject -Compress
    Send-Message -client $client -message $jsonMessage
    $response = Receive-Message -client $client

    $responseObject = ConvertFrom-Json -InputObject $response

    if ($responseObject.Status -eq 0) {
        Write-Host -ForegroundColor Green "Operation successful: $($responseObject.ResponseBody)"
    } else {
        Write-Host -ForegroundColor Red "Operation failed: $($responseObject.ResponseBody)"
    }

    if ($responseObject.Request.RequestType -eq 3 -and $responseObject.ResponseBody -ne $null) {
        Write-Host -ForegroundColor Yellow "Query Result:"

        # Convertir la cadena CSV en una lista de objetos para formatear como tabla
        $rows = $responseObject.ResponseBody -split ","
        $data = @()

        # La primera fila son los nombres de las columnas
        $columns = $rows[0..4]

        # Iterar sobre las demás filas de datos
        for ($i = 5; $i -lt $rows.Length; $i += 5) {
            $data += [PSCustomObject]@{
                ID = $rows[$i]
                Nombre = $rows[$i + 1]
                Apellido1 = $rows[$i + 2]
                Apellido2 = $rows[$i + 3]
                FechaNacimiento = $rows[$i + 4]
            }
        }

        # Mostrar la tabla usando los nombres de columnas de la primera fila
        $data | Format-Table -Property $columns -AutoSize
    }

    if ($responseObject.Data -ne $null) {
        Write-Host -ForegroundColor Yellow "Additional Data: $($responseObject.Data)"
    }

    $client.Shutdown([System.Net.Sockets.SocketShutdown]::Both)
    $client.Close()
}

function Receive-Message {
    param (
        [Parameter(Mandatory = $true)]
        [System.Net.Sockets.Socket]$client
    )

    $stream = New-Object System.Net.Sockets.NetworkStream($client)
    $reader = New-Object System.IO.StreamReader($stream)
    
    try {
        $response = $reader.ReadLine()  # Leer la respuesta del servidor
        return $response
    }
    finally {
        $reader.Close()
        $stream.Close()
    }
}


# Solicitar consulta SQL del usuario de forma interactiva
while ($true) {
    $command = Read-Host "Introduce una consulta SQL (o escribe 'salir' para terminar)"
    
    if ($command -eq "salir") {
        break
    }

    Send-SQLCommand -command $command
}

# Ejemplos de uso
#INSERT INTO Estudiantes VALUES (1, 'Pizza', 'Lunes');
#SELECT * FROM comidas
#DELETE FROM comidas WHERE ID = 1
#CREATE TABLE Estudiantes (ID INT, Nombre, Apellido )
#CREATE DATABASE Escuela;
#SET DATABASE Escuela;

#Ejemplos de uso 2
#CREATE DATABASE Universidad;
#SET DATABASE Universidad;
#CREATE TABLE Estudiante (ID INT, Nombre VARCHAR(30), Apellido1 VARCHAR(30), Apellido2 VARCHAR(30), FechaNacimiento DATETIME);a
#INSERT INTO Estudiante VALUES (1, "Isaac", "Ramirez", "Herrera", "2000-01-01 01:02:00")
#INSERT INTO Estudiante VALUES (2, "Juan", "Perez", "Gonzalez", "2001-02-02 02:03:00")
#INSERT INTO Estudiante VALUES (3, "Maria", "Gonzalez", "Hernandez", "2002-03-03 03:04:00")
#DELETE FROM Estudiante WHERE ID = 1
#SELECT * FROM Estudiante