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
    
    # Verifica el tipo de comando para el RequestType
    $requestType = if ($command -match "^INSERT") { 1 }
    elseif ($command -match "^DELETE") { 2 }
    elseif ($command -match "^SELECT") { 3 }
    else { 0 }  # 0 for invalid/unknown commands

    $requestObject = [PSCustomObject]@{
        RequestType = $requestType;
        RequestBody = $command
    }

    Write-Host -ForegroundColor Green "Sending command: $command"
    
    $jsonMessage = ConvertTo-Json -InputObject $requestObject -Compress
    Send-Message -client $client -message $jsonMessage
    $response = Receive-Message -client $client

    # Convertimos la respuesta JSON en un objeto PowerShell
    $responseObject = ConvertFrom-Json -InputObject $response
    
    # Mostrar solo información relevante (éxito o fallo)
    if ($responseObject.Status -eq 0) {
        Write-Host -ForegroundColor Green "Operation successful: $($responseObject.ResponseBody)"
    } else {
        Write-Host -ForegroundColor Red "Operation failed: $($responseObject.ResponseBody)"
    }

    # Si es un SELECT, mostrar los datos en formato tabla
    if ($responseObject.Request.RequestType -eq 3 -and $responseObject.ResponseBody -ne $null) {
        Write-Host -ForegroundColor Yellow "Query Result:"
        
        # Convertir la cadena CSV en una lista de objetos para formatear como tabla
        $rows = $responseObject.ResponseBody -split ","
        $data = @()
        
        for ($i = 0; $i -lt $rows.Length; $i += 3) {
            $data += [PSCustomObject]@{
                ID = $rows[$i]
                Comida = $rows[$i + 1]
                Dia = $rows[$i + 2]
            }
        }
        
        # Mostrar la tabla
        $data | Format-Table -AutoSize
    }

    # Si hay datos adicionales (aunque no sea un SELECT), mostrarlos también
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
#CREATE TABLE Estudiantes (ID, Nombre, Apellido )