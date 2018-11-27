set arg1=%1
set arg2=%2
set OPENSSL_CONF=openssl.cnf 
"../bin/openssl" req -new -out "../certificates/local/side.req" -key "../certificates/ca/sideCA.key" -passin pass:%arg2% -subj /CN=\%arg1%
"../bin/openssl" x509 -req -passin pass:%arg2% -sha256 -in "../certificates/local/side.req" -out "../certificates/local/side.cer" -CAkey "../certificates/ca/sideCA.key" -CA "../certificates/ca/sideCA.cer" -days 365 -CAcreateserial -CAserial "../certificates/local/serial"
"../bin/openssl" pkcs12 -passin pass:%arg2% -inkey "../certificates/ca/sideCA.key" -in "../certificates/local/side.cer" -export -out "../certificates/local/side.pfx" -password pass:4IPA_only!
