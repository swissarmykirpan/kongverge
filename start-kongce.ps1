$kongdb_name = 'kong-ce-database'

#Start DB Server
$dockerOut = docker ps -a | Select-String $kongdb_name
if ($dockerOut -match $kongdb_name) {
    if ($dockerOut -match ' Up ') {
        docker stop $kongdb_name
    }
    
    Write-Host -NoNewline "Removing stale DB server "
    docker rm $kongdb_name
}

Write-Host "Starting clean DB server"
docker run -d --name kong-ce-database -p 5432:5432 -e "POSTGRES_USER=kong" -e "POSTGRES_DB=kong" postgres:9.5
Start-Sleep -s 10

Write-Host "Running DB migrations"
docker run --rm --link kong-ce-database:kong-ce-database -e "KONG_DATABASE=postgres" -e "KONG_PG_HOST=kong-ce-database" -e "KONG_CASSANDRA_CONTACT_POINTS=kong-ce-database" kong kong migrations up


$dockerOut = docker ps -a -f "ancestor=kong" | Select-String 'kong '
if ($dockerOut -match 'kong ') {
    Write-Host "Removing stale Kong server"
    if ($dockerOut -match ' Up ') {
        docker stop 'kong'
    }
    docker rm 'kong'
}

Write-Host "Starting Kong Server"
docker run -d --name kong --link kong-ce-database:kong-ce-database -e "KONG_DATABASE=postgres" -e "KONG_PG_HOST=kong-ce-database" -e "KONG_CASSANDRA_CONTACT_POINTS=kong-ce-database" -e "KONG_PROXY_ACCESS_LOG=/dev/stdout" -e "KONG_ADMIN_ACCESS_LOG=/dev/stdout" -e "KONG_PROXY_ERROR_LOG=/dev/stderr" -e "KONG_ADMIN_ERROR_LOG=/dev/stderr" -e "KONG_VITALS=on" -e "KONG_ADMIN_LISTEN=0.0.0.0:8001" -e "KONG_PORTAL=on" -e "KONG_PORTAL_GUI_URI=localhost:8003" -p 8000:8000 -p 8443:8443 -p 8001:8001 -p 8444:8444 -p 8445:8445 kong
