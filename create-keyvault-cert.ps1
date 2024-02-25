# Login to Azure
Connect-AzAccount
Set-AzContext -Subscription "4d49c197-62e6-4d24-93a1-b8d000df1603"

# Variables
$keyVaultName="sundmankv1"
$certificateName="cert2"
$subjectName="CN=app2.it-total.internal"
$appName="sundmanapp2"

# Define the certificate police
$certificatePolicy = New-AzKeyVaultCertificatePolicy `
    -SubjectName $subjectName `
    -IssuerName "Self" `
    -ValidityInMonths 108 `
    -KeyUsage "DigitalSignature" `
    -EmailAtNumberOfDaysBeforeExpiry 30 `
    -SecretContentType "application/x-pkcs12" `
    -KeyNotExportable

# Create the certificate and get the base64 encoded certificate
Add-AzKeyVaultCertificate -VaultName $keyVaultName -Name $certificateName -CertificatePolicy $certificatePolicy
$certBase64 = Get-AzKeyVaultSecret -VaultName $keyVaultName -Name $certificateName -AsPlainText

# Create the application (in remote tenant)
New-AzADApplication -DisplayName $appName -CertValue $certBase64 -EndDate (Get-Date).AddYears(9)

# Use token with Connect-MgGraph
$secureAccessToken = $plainAccessToken | ConvertTo-SecureString -AsPlainText -Force
Connect-MgGraph -AccessToken $secureAccessToken  