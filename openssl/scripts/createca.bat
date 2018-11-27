set arg1=%1
set OPENSSL_CONF=openssl.cnf 
"../bin/openssl" genrsa -out "../certificates/ca/sideCA.key" -passout pass:%arg1% -aes128  2048
"../bin/openssl" req -x509 -sha256 -new -key "../certificates/ca/sideCA.key" -out "../certificates/ca/sideCA.cer" -days 730 -passin pass:%arg1% -subj /CN="sideloader ca" 