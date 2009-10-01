Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Configuration

' A simple, string-oriented wrapper class for encryption functions, including 
' Hashing, Symmetric Encryption, and Asymmetric Encryption.
'
'  Jeff Atwood
'   http://www.codinghorror.com/

Namespace Encryption

#Region "  Hash"

    ''' <summary>
    ''' Hash functions are fundamental to modern cryptography. These functions map binary 
    ''' strings of an arbitrary length to small binary strings of a fixed length, known as 
    ''' hash values. A cryptographic hash function has the property that it is computationally
    ''' infeasible to find two distinct inputs that hash to the same value. Hash functions 
    ''' are commonly used with digital signatures and for data integrity.
    ''' </summary>
    Public Class Hash

        ''' <summary>
        ''' Type of hash; some are security oriented, others are fast and simple
        ''' </summary>
        Public Enum Provider
            ''' <summary>
            ''' Cyclic Redundancy Check provider, 32-bit
            ''' </summary>
            CRC32
            ''' <summary>
            ''' Secure Hashing Algorithm provider, SHA-1 variant, 160-bit
            ''' </summary>
            SHA1
            ''' <summary>
            ''' Secure Hashing Algorithm provider, SHA-2 variant, 256-bit
            ''' </summary>
            SHA256
            ''' <summary>
            ''' Secure Hashing Algorithm provider, SHA-2 variant, 384-bit
            ''' </summary>
            SHA384
            ''' <summary>
            ''' Secure Hashing Algorithm provider, SHA-2 variant, 512-bit
            ''' </summary>
            SHA512
            ''' <summary>
            ''' Message Digest algorithm 5, 128-bit
            ''' </summary>
            MD5
        End Enum

        Private _Hash As HashAlgorithm
        Private _HashValue As New Data

        Private Sub New()
        End Sub

        ''' <summary>
        ''' Instantiate a new hash of the specified type
        ''' </summary>
        Public Sub New(ByVal p As Provider)
            Select Case p
                Case Provider.CRC32
                    _Hash = New CRC32
                Case Provider.MD5
                    _Hash = New MD5CryptoServiceProvider
                Case Provider.SHA1
                    _Hash = New SHA1Managed
                Case Provider.SHA256
                    _Hash = New SHA256Managed
                Case Provider.SHA384
                    _Hash = New SHA384Managed
                Case Provider.SHA512
                    _Hash = New SHA512Managed
            End Select
        End Sub

        ''' <summary>
        ''' Returns the previously calculated hash
        ''' </summary>
        Public ReadOnly Property Value() As Data
            Get
                Return _HashValue
            End Get
        End Property

        ''' <summary>
        ''' Calculates hash on a stream of arbitrary length
        ''' </summary>
        Public Function Calculate(ByRef s As System.IO.Stream) As Data
            _HashValue.Bytes = _Hash.ComputeHash(s)
            Return _HashValue
        End Function

        ''' <summary>
        ''' Calculates hash for fixed length <see cref="Data"/>
        ''' </summary>
        Public Function Calculate(ByVal d As Data) As Data
            Return CalculatePrivate(d.Bytes)
        End Function

        ''' <summary>
        ''' Calculates hash for a string with a prefixed salt value. 
        ''' A "salt" is random data prefixed to every hashed value to prevent 
        ''' common dictionary attacks.
        ''' </summary>
        Public Function Calculate(ByVal d As Data, ByVal salt As Data) As Data
            Dim nb(d.Bytes.Length + salt.Bytes.Length - 1) As Byte
            salt.Bytes.CopyTo(nb, 0)
            d.Bytes.CopyTo(nb, salt.Bytes.Length)
            Return CalculatePrivate(nb)
        End Function

        ''' <summary>
        ''' Calculates hash for an array of bytes
        ''' </summary>
        Private Function CalculatePrivate(ByVal b() As Byte) As Data
            _HashValue.Bytes = _Hash.ComputeHash(b)
            Return _HashValue
        End Function

#Region "  CRC32 HashAlgorithm"
        Private Class CRC32
            Inherits HashAlgorithm

            Private result As Integer = &HFFFFFFFF

            Protected Overrides Sub HashCore(ByVal array() As Byte, ByVal ibStart As Integer, ByVal cbSize As Integer)
                Dim lookup As Integer
                For i As Integer = ibStart To cbSize - 1
                    lookup = (result And &HFF) Xor array(i)
                    result = ((result And &HFFFFFF00) \ &H100) And &HFFFFFF
                    result = result Xor crcLookup(lookup)
                Next i
            End Sub

            Protected Overrides Function HashFinal() As Byte()
                Dim b() As Byte = BitConverter.GetBytes(Not result)
                Array.Reverse(b)
                Return b
            End Function

            Public Overrides Sub Initialize()
                result = &HFFFFFFFF
            End Sub

            Private crcLookup() As Integer = { _
                &H0, &H77073096, &HEE0E612C, &H990951BA, _
                &H76DC419, &H706AF48F, &HE963A535, &H9E6495A3, _
                &HEDB8832, &H79DCB8A4, &HE0D5E91E, &H97D2D988, _
                &H9B64C2B, &H7EB17CBD, &HE7B82D07, &H90BF1D91, _
                &H1DB71064, &H6AB020F2, &HF3B97148, &H84BE41DE, _
                &H1ADAD47D, &H6DDDE4EB, &HF4D4B551, &H83D385C7, _
                &H136C9856, &H646BA8C0, &HFD62F97A, &H8A65C9EC, _
                &H14015C4F, &H63066CD9, &HFA0F3D63, &H8D080DF5, _
                &H3B6E20C8, &H4C69105E, &HD56041E4, &HA2677172, _
                &H3C03E4D1, &H4B04D447, &HD20D85FD, &HA50AB56B, _
                &H35B5A8FA, &H42B2986C, &HDBBBC9D6, &HACBCF940, _
                &H32D86CE3, &H45DF5C75, &HDCD60DCF, &HABD13D59, _
                &H26D930AC, &H51DE003A, &HC8D75180, &HBFD06116, _
                &H21B4F4B5, &H56B3C423, &HCFBA9599, &HB8BDA50F, _
                &H2802B89E, &H5F058808, &HC60CD9B2, &HB10BE924, _
                &H2F6F7C87, &H58684C11, &HC1611DAB, &HB6662D3D, _
                &H76DC4190, &H1DB7106, &H98D220BC, &HEFD5102A, _
                &H71B18589, &H6B6B51F, &H9FBFE4A5, &HE8B8D433, _
                &H7807C9A2, &HF00F934, &H9609A88E, &HE10E9818, _
                &H7F6A0DBB, &H86D3D2D, &H91646C97, &HE6635C01, _
                &H6B6B51F4, &H1C6C6162, &H856530D8, &HF262004E, _
                &H6C0695ED, &H1B01A57B, &H8208F4C1, &HF50FC457, _
                &H65B0D9C6, &H12B7E950, &H8BBEB8EA, &HFCB9887C, _
                &H62DD1DDF, &H15DA2D49, &H8CD37CF3, &HFBD44C65, _
                &H4DB26158, &H3AB551CE, &HA3BC0074, &HD4BB30E2, _
                &H4ADFA541, &H3DD895D7, &HA4D1C46D, &HD3D6F4FB, _
                &H4369E96A, &H346ED9FC, &HAD678846, &HDA60B8D0, _
                &H44042D73, &H33031DE5, &HAA0A4C5F, &HDD0D7CC9, _
                &H5005713C, &H270241AA, &HBE0B1010, &HC90C2086, _
                &H5768B525, &H206F85B3, &HB966D409, &HCE61E49F, _
                &H5EDEF90E, &H29D9C998, &HB0D09822, &HC7D7A8B4, _
                &H59B33D17, &H2EB40D81, &HB7BD5C3B, &HC0BA6CAD, _
                &HEDB88320, &H9ABFB3B6, &H3B6E20C, &H74B1D29A, _
                &HEAD54739, &H9DD277AF, &H4DB2615, &H73DC1683, _
                &HE3630B12, &H94643B84, &HD6D6A3E, &H7A6A5AA8, _
                &HE40ECF0B, &H9309FF9D, &HA00AE27, &H7D079EB1, _
                &HF00F9344, &H8708A3D2, &H1E01F268, &H6906C2FE, _
                &HF762575D, &H806567CB, &H196C3671, &H6E6B06E7, _
                &HFED41B76, &H89D32BE0, &H10DA7A5A, &H67DD4ACC, _
                &HF9B9DF6F, &H8EBEEFF9, &H17B7BE43, &H60B08ED5, _
                &HD6D6A3E8, &HA1D1937E, &H38D8C2C4, &H4FDFF252, _
                &HD1BB67F1, &HA6BC5767, &H3FB506DD, &H48B2364B, _
                &HD80D2BDA, &HAF0A1B4C, &H36034AF6, &H41047A60, _
                &HDF60EFC3, &HA867DF55, &H316E8EEF, &H4669BE79, _
                &HCB61B38C, &HBC66831A, &H256FD2A0, &H5268E236, _
                &HCC0C7795, &HBB0B4703, &H220216B9, &H5505262F, _
                &HC5BA3BBE, &HB2BD0B28, &H2BB45A92, &H5CB36A04, _
                &HC2D7FFA7, &HB5D0CF31, &H2CD99E8B, &H5BDEAE1D, _
                &H9B64C2B0, &HEC63F226, &H756AA39C, &H26D930A, _
                &H9C0906A9, &HEB0E363F, &H72076785, &H5005713, _
                &H95BF4A82, &HE2B87A14, &H7BB12BAE, &HCB61B38, _
                &H92D28E9B, &HE5D5BE0D, &H7CDCEFB7, &HBDBDF21, _
                &H86D3D2D4, &HF1D4E242, &H68DDB3F8, &H1FDA836E, _
                &H81BE16CD, &HF6B9265B, &H6FB077E1, &H18B74777, _
                &H88085AE6, &HFF0F6A70, &H66063BCA, &H11010B5C, _
                &H8F659EFF, &HF862AE69, &H616BFFD3, &H166CCF45, _
                &HA00AE278, &HD70DD2EE, &H4E048354, &H3903B3C2, _
                &HA7672661, &HD06016F7, &H4969474D, &H3E6E77DB, _
                &HAED16A4A, &HD9D65ADC, &H40DF0B66, &H37D83BF0, _
                &HA9BCAE53, &HDEBB9EC5, &H47B2CF7F, &H30B5FFE9, _
                &HBDBDF21C, &HCABAC28A, &H53B39330, &H24B4A3A6, _
                &HBAD03605, &HCDD70693, &H54DE5729, &H23D967BF, _
                &HB3667A2E, &HC4614AB8, &H5D681B02, &H2A6F2B94, _
                &HB40BBE37, &HC30C8EA1, &H5A05DF1B, &H2D02EF8D}

            Public Overrides ReadOnly Property Hash() As Byte()
                Get
                    Dim b() As Byte = BitConverter.GetBytes(Not result)
                    Array.Reverse(b)
                    Return b
                End Get
            End Property
        End Class

#End Region

    End Class
#End Region

#Region "  Symmetric"

    ''' <summary>
    ''' Symmetric encryption uses a single key to encrypt and decrypt. 
    ''' Both parties (encryptor and decryptor) must share the same secret key.
    ''' </summary>
    Public Class Symmetric

        Private Const _DefaultIntializationVector As String = "%1Az=-@qT"
        Private Const _BufferSize As Integer = 2048

        Public Enum Provider
            ''' <summary>
            ''' The Data Encryption Standard provider supports a 64 bit key only
            ''' </summary>
            DES
            ''' <summary>
            ''' The Rivest Cipher 2 provider supports keys ranging from 40 to 128 bits, default is 128 bits
            ''' </summary>
            RC2
            ''' <summary>
            ''' The Rijndael (also known as AES) provider supports keys of 128, 192, or 256 bits with a default of 256 bits
            ''' </summary>
            Rijndael
            ''' <summary>
            ''' The TripleDES provider (also known as 3DES) supports keys of 128 or 192 bits with a default of 192 bits
            ''' </summary>
            TripleDES
        End Enum

        Private _data As Data
        Private _key As Data
        Private _iv As Data
        Private _crypto As SymmetricAlgorithm
        Private _EncryptedBytes As Byte()
        Private _UseDefaultInitializationVector As Boolean

        Private Sub New()
        End Sub

        ''' <summary>
        ''' Instantiates a new symmetric encryption object using the specified provider.
        ''' </summary>
        Public Sub New(ByVal provider As Provider, Optional ByVal useDefaultInitializationVector As Boolean = True)
            Select Case provider
                Case provider.DES
                    _crypto = New DESCryptoServiceProvider
                Case provider.RC2
                    _crypto = New RC2CryptoServiceProvider
                Case provider.Rijndael
                    _crypto = New RijndaelManaged
                Case provider.TripleDES
                    _crypto = New TripleDESCryptoServiceProvider
            End Select

            '-- make sure key and IV are always set, no matter what
            Me.Key = RandomKey()
            If useDefaultInitializationVector Then
                Me.IntializationVector = New Data(_DefaultIntializationVector)
            Else
                Me.IntializationVector = RandomInitializationVector()
            End If
        End Sub

        ''' <summary>
        ''' Key size in bytes. We use the default key size for any given provider; if you 
        ''' want to force a specific key size, set this property
        ''' </summary>
        Public Property KeySizeBytes() As Integer
            Get
                Return _crypto.KeySize \ 8
            End Get
            Set(ByVal Value As Integer)
                _crypto.KeySize = Value * 8
                _key.MaxBytes = Value
            End Set
        End Property

        ''' <summary>
        ''' Key size in bits. We use the default key size for any given provider; if you 
        ''' want to force a specific key size, set this property
        ''' </summary>
        Public Property KeySizeBits() As Integer
            Get
                Return _crypto.KeySize
            End Get
            Set(ByVal Value As Integer)
                _crypto.KeySize = Value
                _key.MaxBits = Value
            End Set
        End Property

        ''' <summary>
        ''' The key used to encrypt/decrypt data
        ''' </summary>
        Public Property Key() As Data
            Get
                Return _key
            End Get
            Set(ByVal Value As Data)
                _key = Value
                _key.MaxBytes = _crypto.LegalKeySizes(0).MaxSize \ 8
                _key.MinBytes = _crypto.LegalKeySizes(0).MinSize \ 8
                _key.StepBytes = _crypto.LegalKeySizes(0).SkipSize \ 8
            End Set
        End Property

        ''' <summary>
        ''' Using the default Cipher Block Chaining (CBC) mode, all data blocks are processed using
        ''' the value derived from the previous block; the first data block has no previous data block
        ''' to use, so it needs an InitializationVector to feed the first block
        ''' </summary>
        Public Property IntializationVector() As Data
            Get
                Return _iv
            End Get
            Set(ByVal Value As Data)
                _iv = Value
                _iv.MaxBytes = _crypto.BlockSize \ 8
                _iv.MinBytes = _crypto.BlockSize \ 8
            End Set
        End Property

        ''' <summary>
        ''' generates a random Initialization Vector, if one was not provided
        ''' </summary>
        Public Function RandomInitializationVector() As Data
            _crypto.GenerateIV()
            Dim d As New Data(_crypto.IV)
            Return d
        End Function

        ''' <summary>
        ''' generates a random Key, if one was not provided
        ''' </summary>
        Public Function RandomKey() As Data
            _crypto.GenerateKey()
            Dim d As New Data(_crypto.Key)
            Return d
        End Function

        ''' <summary>
        ''' Ensures that _crypto object has valid Key and IV
        ''' prior to any attempt to encrypt/decrypt anything
        ''' </summary>
        Private Sub ValidateKeyAndIv(ByVal isEncrypting As Boolean)
            If _key.IsEmpty Then
                If isEncrypting Then
                    _key = RandomKey()
                Else
                    Throw New CryptographicException("No key was provided for the decryption operation!")
                End If
            End If
            If _iv.IsEmpty Then
                If isEncrypting Then
                    _iv = RandomInitializationVector()
                Else
                    Throw New CryptographicException("No initialization vector was provided for the decryption operation!")
                End If
            End If
            _crypto.Key = _key.Bytes
            _crypto.IV = _iv.Bytes
        End Sub

        ''' <summary>
        ''' Encrypts the specified Data using provided key
        ''' </summary>
        Public Function Encrypt(ByVal d As Data, ByVal key As Data) As Data
            Me.Key = key
            Return Encrypt(d)
        End Function

        ''' <summary>
        ''' Encrypts the specified Data using preset key and preset initialization vector
        ''' </summary>
        Public Function Encrypt(ByVal d As Data) As Data
            Dim ms As New IO.MemoryStream

            ValidateKeyAndIv(True)

            Dim cs As New CryptoStream(ms, _crypto.CreateEncryptor(), CryptoStreamMode.Write)
            cs.Write(d.Bytes, 0, d.Bytes.Length)
            cs.Close()
            ms.Close()

            Return New Data(ms.ToArray)
        End Function

        ''' <summary>
        ''' Encrypts the stream to memory using provided key and provided initialization vector
        ''' </summary>
        Public Function Encrypt(ByVal s As Stream, ByVal key As Data, ByVal iv As Data) As Data
            Me.IntializationVector = iv
            Me.Key = key
            Return Encrypt(s)
        End Function

        ''' <summary>
        ''' Encrypts the stream to memory using specified key
        ''' </summary>
        Public Function Encrypt(ByVal s As Stream, ByVal key As Data) As Data
            Me.Key = key
            Return Encrypt(s)
        End Function

        ''' <summary>
        ''' Encrypts the specified stream to memory using preset key and preset initialization vector
        ''' </summary>
        Public Function Encrypt(ByVal s As Stream) As Data
            Dim ms As New IO.MemoryStream
            Dim b(_BufferSize) As Byte
            Dim i As Integer

            ValidateKeyAndIv(True)

            Dim cs As New CryptoStream(ms, _crypto.CreateEncryptor(), CryptoStreamMode.Write)
            i = s.Read(b, 0, _BufferSize)
            Do While i > 0
                cs.Write(b, 0, i)
                i = s.Read(b, 0, _BufferSize)
            Loop

            cs.Close()
            ms.Close()

            Return New Data(ms.ToArray)
        End Function

        ''' <summary>
        ''' Decrypts the specified data using provided key and preset initialization vector
        ''' </summary>
        Public Function Decrypt(ByVal encryptedData As Data, ByVal key As Data) As Data
            Me.Key = key
            Return Decrypt(encryptedData)
        End Function

        ''' <summary>
        ''' Decrypts the specified stream using provided key and preset initialization vector
        ''' </summary>
        Public Function Decrypt(ByVal encryptedStream As Stream, ByVal key As Data) As Data
            Me.Key = key
            Return Decrypt(encryptedStream)
        End Function

        ''' <summary>
        ''' Decrypts the specified stream using preset key and preset initialization vector
        ''' </summary>
        Public Function Decrypt(ByVal encryptedStream As Stream) As Data
            Dim ms As New System.IO.MemoryStream
            Dim b(_BufferSize) As Byte

            ValidateKeyAndIv(False)
            Dim cs As New CryptoStream(encryptedStream, _
                _crypto.CreateDecryptor(), CryptoStreamMode.Read)

            Dim i As Integer
            i = cs.Read(b, 0, _BufferSize)

            Do While i > 0
                ms.Write(b, 0, i)
                i = cs.Read(b, 0, _BufferSize)
            Loop
            cs.Close()
            ms.Close()

            Return New Data(ms.ToArray)
        End Function

        ''' <summary>
        ''' Decrypts the specified data using preset key and preset initialization vector
        ''' </summary>
        Public Function Decrypt(ByVal encryptedData As Data) As Data
            Dim ms As New System.IO.MemoryStream(encryptedData.Bytes, 0, encryptedData.Bytes.Length)
            Dim b() As Byte = New Byte(encryptedData.Bytes.Length - 1) {}

            ValidateKeyAndIv(False)
            Dim cs As New CryptoStream(ms, _crypto.CreateDecryptor(), CryptoStreamMode.Read)

            Try
                cs.Read(b, 0, encryptedData.Bytes.Length - 1)
            Catch ex As CryptographicException
                Throw New CryptographicException("Unable to decrypt data. The provided key may be invalid.", ex)
            Finally
                cs.Close()
            End Try
            Return New Data(b)
        End Function

    End Class

#End Region

#Region "  Asymmetric"

    ''' <summary>
    ''' Asymmetric encryption uses a pair of keys to encrypt and decrypt.
    ''' There is a "public" key which is used to encrypt. Decrypting, on the other hand, 
    ''' requires both the "public" key and an additional "private" key. The advantage is 
    ''' that people can send you encrypted messages without being able to decrypt them.
    ''' </summary>
    ''' <remarks>
    ''' The only provider supported is the <see cref="RSACryptoServiceProvider"/>
    ''' </remarks>
    Public Class Asymmetric

        Private _rsa As RSACryptoServiceProvider
        Private _KeyContainerName As String = "Encryption.AsymmetricEncryption.DefaultContainerName"
        Private _UseMachineKeystore As Boolean = True
        Private _KeySize As Integer = 1024

        Private Const _ElementParent As String = "RSAKeyValue"
        Private Const _ElementModulus As String = "Modulus"
        Private Const _ElementExponent As String = "Exponent"
        Private Const _ElementPrimeP As String = "P"
        Private Const _ElementPrimeQ As String = "Q"
        Private Const _ElementPrimeExponentP As String = "DP"
        Private Const _ElementPrimeExponentQ As String = "DQ"
        Private Const _ElementCoefficient As String = "InverseQ"
        Private Const _ElementPrivateExponent As String = "D"

        '-- http://forum.java.sun.com/thread.jsp?forum=9&thread=552022&tstart=0&trange=15 
        Private Const _KeyModulus As String = "PublicKey.Modulus"
        Private Const _KeyExponent As String = "PublicKey.Exponent"
        Private Const _KeyPrimeP As String = "PrivateKey.P"
        Private Const _KeyPrimeQ As String = "PrivateKey.Q"
        Private Const _KeyPrimeExponentP As String = "PrivateKey.DP"
        Private Const _KeyPrimeExponentQ As String = "PrivateKey.DQ"
        Private Const _KeyCoefficient As String = "PrivateKey.InverseQ"
        Private Const _KeyPrivateExponent As String = "PrivateKey.D"

#Region "  PublicKey Class"
        ''' <summary>
        ''' Represents a public encryption key. Intended to be shared, it 
        ''' contains only the Modulus and Exponent.
        ''' </summary>
        Public Class PublicKey
            Public Modulus As String
            Public Exponent As String

            Public Sub New()
            End Sub

            Public Sub New(ByVal KeyXml As String)
                LoadFromXml(KeyXml)
            End Sub

            ''' <summary>
            ''' Load public key from App.config or Web.config file
            ''' </summary>
            Public Sub LoadFromConfig()
                Me.Modulus = Utils.GetConfigString(_KeyModulus)
                Me.Exponent = Utils.GetConfigString(_KeyExponent)
            End Sub

            ''' <summary>
            ''' Returns *.config file XML section representing this public key
            ''' </summary>
            Public Function ToConfigSection() As String
                Dim sb As New StringBuilder
                With sb
                    .Append(Utils.WriteConfigKey(_KeyModulus, Me.Modulus))
                    .Append(Utils.WriteConfigKey(_KeyExponent, Me.Exponent))
                End With
                Return sb.ToString
            End Function

            ''' <summary>
            ''' Writes the *.config file representation of this public key to a file
            ''' </summary>
            Public Sub ExportToConfigFile(ByVal filePath As String)
                Dim sw As New StreamWriter(filePath, False)
                sw.Write(Me.ToConfigSection)
                sw.Close()
            End Sub

            ''' <summary>
            ''' Loads the public key from its XML string
            ''' </summary>
            Public Sub LoadFromXml(ByVal keyXml As String)
                Me.Modulus = Utils.GetXmlElement(keyXml, "Modulus")
                Me.Exponent = Utils.GetXmlElement(keyXml, "Exponent")
            End Sub

            ''' <summary>
            ''' Converts this public key to an RSAParameters object
            ''' </summary>
            Public Function ToParameters() As RSAParameters
                Dim r As New RSAParameters
                r.Modulus = Convert.FromBase64String(Me.Modulus)
                r.Exponent = Convert.FromBase64String(Me.Exponent)
                Return r
            End Function

            ''' <summary>
            ''' Converts this public key to its XML string representation
            ''' </summary>
            Public Function ToXml() As String
                Dim sb As New StringBuilder
                With sb
                    .Append(Utils.WriteXmlNode(_ElementParent))
                    .Append(Utils.WriteXmlElement(_ElementModulus, Me.Modulus))
                    .Append(Utils.WriteXmlElement(_ElementExponent, Me.Exponent))
                    .Append(Utils.WriteXmlNode(_ElementParent, True))
                End With
                Return sb.ToString
            End Function

            ''' <summary>
            ''' Writes the Xml representation of this public key to a file
            ''' </summary>
            Public Sub ExportToXmlFile(ByVal filePath As String)
                Dim sw As New StreamWriter(filePath, False)
                sw.Write(Me.ToXml)
                sw.Close()
            End Sub

        End Class
#End Region

#Region "  PrivateKey Class"

        ''' <summary>
        ''' Represents a private encryption key. Not intended to be shared, as it 
        ''' contains all the elements that make up the key.
        ''' </summary>
        Public Class PrivateKey
            Public Modulus As String
            Public Exponent As String
            Public PrimeP As String
            Public PrimeQ As String
            Public PrimeExponentP As String
            Public PrimeExponentQ As String
            Public Coefficient As String
            Public PrivateExponent As String

            Public Sub New()
            End Sub

            Public Sub New(ByVal keyXml As String)
                LoadFromXml(keyXml)
            End Sub

            ''' <summary>
            ''' Load private key from App.config or Web.config file
            ''' </summary>
            Public Sub LoadFromConfig()
                Me.Modulus = Utils.GetConfigString(_KeyModulus)
                Me.Exponent = Utils.GetConfigString(_KeyExponent)
                Me.PrimeP = Utils.GetConfigString(_KeyPrimeP)
                Me.PrimeQ = Utils.GetConfigString(_KeyPrimeQ)
                Me.PrimeExponentP = Utils.GetConfigString(_KeyPrimeExponentP)
                Me.PrimeExponentQ = Utils.GetConfigString(_KeyPrimeExponentQ)
                Me.Coefficient = Utils.GetConfigString(_KeyCoefficient)
                Me.PrivateExponent = Utils.GetConfigString(_KeyPrivateExponent)
            End Sub

            ''' <summary>
            ''' Converts this private key to an RSAParameters object
            ''' </summary>
            Public Function ToParameters() As RSAParameters
                Dim r As New RSAParameters
                r.Modulus = Convert.FromBase64String(Me.Modulus)
                r.Exponent = Convert.FromBase64String(Me.Exponent)
                r.P = Convert.FromBase64String(Me.PrimeP)
                r.Q = Convert.FromBase64String(Me.PrimeQ)
                r.DP = Convert.FromBase64String(Me.PrimeExponentP)
                r.DQ = Convert.FromBase64String(Me.PrimeExponentQ)
                r.InverseQ = Convert.FromBase64String(Me.Coefficient)
                r.D = Convert.FromBase64String(Me.PrivateExponent)
                Return r
            End Function

            ''' <summary>
            ''' Returns *.config file XML section representing this private key
            ''' </summary>
            Public Function ToConfigSection() As String
                Dim sb As New StringBuilder
                With sb
                    .Append(Utils.WriteConfigKey(_KeyModulus, Me.Modulus))
                    .Append(Utils.WriteConfigKey(_KeyExponent, Me.Exponent))
                    .Append(Utils.WriteConfigKey(_KeyPrimeP, Me.PrimeP))
                    .Append(Utils.WriteConfigKey(_KeyPrimeQ, Me.PrimeQ))
                    .Append(Utils.WriteConfigKey(_KeyPrimeExponentP, Me.PrimeExponentP))
                    .Append(Utils.WriteConfigKey(_KeyPrimeExponentQ, Me.PrimeExponentQ))
                    .Append(Utils.WriteConfigKey(_KeyCoefficient, Me.Coefficient))
                    .Append(Utils.WriteConfigKey(_KeyPrivateExponent, Me.PrivateExponent))
                End With
                Return sb.ToString
            End Function

            ''' <summary>
            ''' Writes the *.config file representation of this private key to a file
            ''' </summary>
            Public Sub ExportToConfigFile(ByVal strFilePath As String)
                Dim sw As New StreamWriter(strFilePath, False)
                sw.Write(Me.ToConfigSection)
                sw.Close()
            End Sub

            ''' <summary>
            ''' Loads the private key from its XML string
            ''' </summary>
            Public Sub LoadFromXml(ByVal keyXml As String)
                Me.Modulus = Utils.GetXmlElement(keyXml, "Modulus")
                Me.Exponent = Utils.GetXmlElement(keyXml, "Exponent")
                Me.PrimeP = Utils.GetXmlElement(keyXml, "P")
                Me.PrimeQ = Utils.GetXmlElement(keyXml, "Q")
                Me.PrimeExponentP = Utils.GetXmlElement(keyXml, "DP")
                Me.PrimeExponentQ = Utils.GetXmlElement(keyXml, "DQ")
                Me.Coefficient = Utils.GetXmlElement(keyXml, "InverseQ")
                Me.PrivateExponent = Utils.GetXmlElement(keyXml, "D")
            End Sub

            ''' <summary>
            ''' Converts this private key to its XML string representation
            ''' </summary>
            Public Function ToXml() As String
                Dim sb As New StringBuilder
                With sb
                    .Append(Utils.WriteXmlNode(_ElementParent))
                    .Append(Utils.WriteXmlElement(_ElementModulus, Me.Modulus))
                    .Append(Utils.WriteXmlElement(_ElementExponent, Me.Exponent))
                    .Append(Utils.WriteXmlElement(_ElementPrimeP, Me.PrimeP))
                    .Append(Utils.WriteXmlElement(_ElementPrimeQ, Me.PrimeQ))
                    .Append(Utils.WriteXmlElement(_ElementPrimeExponentP, Me.PrimeExponentP))
                    .Append(Utils.WriteXmlElement(_ElementPrimeExponentQ, Me.PrimeExponentQ))
                    .Append(Utils.WriteXmlElement(_ElementCoefficient, Me.Coefficient))
                    .Append(Utils.WriteXmlElement(_ElementPrivateExponent, Me.PrivateExponent))
                    .Append(Utils.WriteXmlNode(_ElementParent, True))
                End With
                Return sb.ToString
            End Function

            ''' <summary>
            ''' Writes the Xml representation of this private key to a file
            ''' </summary>
            Public Sub ExportToXmlFile(ByVal filePath As String)
                Dim sw As New StreamWriter(filePath, False)
                sw.Write(Me.ToXml)
                sw.Close()
            End Sub

        End Class

#End Region

        ''' <summary>
        ''' Instantiates a new asymmetric encryption session using the default key size; 
        ''' this is usally 1024 bits
        ''' </summary>
        Public Sub New()
            _rsa = GetRSAProvider()
        End Sub

        ''' <summary>
        ''' Instantiates a new asymmetric encryption session using a specific key size
        ''' </summary>
        Public Sub New(ByVal keySize As Integer)
            _KeySize = keySize
            _rsa = GetRSAProvider()
        End Sub

        ''' <summary>
        ''' Sets the name of the key container used to store this key on disk; this is an 
        ''' unavoidable side effect of the underlying Microsoft CryptoAPI. 
        ''' </summary>
        ''' <remarks>
        ''' http://support.microsoft.com/default.aspx?scid=http://support.microsoft.com:80/support/kb/articles/q322/3/71.asp&amp;NoWebContent=1
        ''' </remarks>
        Public Property KeyContainerName() As String
            Get
                Return _KeyContainerName
            End Get
            Set(ByVal Value As String)
                _KeyContainerName = Value
            End Set
        End Property

        ''' <summary>
        ''' Returns the current key size, in bits
        ''' </summary>
        Public ReadOnly Property KeySizeBits() As Integer
            Get
                Return _rsa.KeySize
            End Get
        End Property

        ''' <summary>
        ''' Returns the maximum supported key size, in bits
        ''' </summary>
        Public ReadOnly Property KeySizeMaxBits() As Integer
            Get
                Return _rsa.LegalKeySizes(0).MaxSize
            End Get
        End Property

        ''' <summary>
        ''' Returns the minimum supported key size, in bits
        ''' </summary>
        Public ReadOnly Property KeySizeMinBits() As Integer
            Get
                Return _rsa.LegalKeySizes(0).MinSize
            End Get
        End Property

        ''' <summary>
        ''' Returns valid key step sizes, in bits
        ''' </summary>
        Public ReadOnly Property KeySizeStepBits() As Integer
            Get
                Return _rsa.LegalKeySizes(0).SkipSize
            End Get
        End Property

        ''' <summary>
        ''' Returns the default public key as stored in the *.config file
        ''' </summary>
        Public ReadOnly Property DefaultPublicKey() As PublicKey
            Get
                Dim pubkey As New PublicKey
                pubkey.LoadFromConfig()
                Return pubkey
            End Get
        End Property

        ''' <summary>
        ''' Returns the default private key as stored in the *.config file
        ''' </summary>
        Public ReadOnly Property DefaultPrivateKey() As PrivateKey
            Get
                Dim privkey As New PrivateKey
                privkey.LoadFromConfig()
                Return privkey
            End Get
        End Property

        ''' <summary>
        ''' Generates a new public/private key pair as objects
        ''' </summary>
        Public Sub GenerateNewKeyset(ByRef publicKey As PublicKey, ByRef privateKey As PrivateKey)
            Dim PublicKeyXML As String = Nothing
            Dim PrivateKeyXML As String = Nothing
            GenerateNewKeyset(PublicKeyXML, PrivateKeyXML)
            publicKey = New PublicKey(PublicKeyXML)
            privateKey = New PrivateKey(PrivateKeyXML)
        End Sub

        ''' <summary>
        ''' Generates a new public/private key pair as XML strings
        ''' </summary>
        Public Sub GenerateNewKeyset(ByRef publicKeyXML As String, ByRef privateKeyXML As String)
            Dim rsa As RSA = RSACryptoServiceProvider.Create
            publicKeyXML = rsa.ToXmlString(False)
            privateKeyXML = rsa.ToXmlString(True)
        End Sub

        ''' <summary>
        ''' Encrypts data using the default public key
        ''' </summary>
        Public Function Encrypt(ByVal d As Data) As Data
            Dim PublicKey As PublicKey = DefaultPublicKey
            Return Encrypt(d, PublicKey)
        End Function

        ''' <summary>
        ''' Encrypts data using the provided public key
        ''' </summary>
        Public Function Encrypt(ByVal d As Data, ByVal publicKey As PublicKey) As Data
            _rsa.ImportParameters(publicKey.ToParameters)
            Return EncryptPrivate(d)
        End Function

        ''' <summary>
        ''' Encrypts data using the provided public key as XML
        ''' </summary>
        Public Function Encrypt(ByVal d As Data, ByVal publicKeyXML As String) As Data
            LoadKeyXml(publicKeyXML, False)
            Return EncryptPrivate(d)
        End Function

        Private Function EncryptPrivate(ByVal d As Data) As Data
            Try
                Return New Data(_rsa.Encrypt(d.Bytes, False))
            Catch ex As CryptographicException
                If ex.Message.ToLower.IndexOf("bad length") > -1 Then
                    Throw New CryptographicException("Your data is too large; RSA encryption is designed to encrypt relatively small amounts of data. The exact byte limit depends on the key size. To encrypt more data, use symmetric encryption and then encrypt that symmetric key with asymmetric RSA encryption.", ex)
                Else
                    Throw
                End If
            End Try
        End Function

        ''' <summary>
        ''' Decrypts data using the default private key
        ''' </summary>
        Public Function Decrypt(ByVal encryptedData As Data) As Data
            Dim PrivateKey As New PrivateKey
            PrivateKey.LoadFromConfig()
            Return Decrypt(encryptedData, PrivateKey)
        End Function

        ''' <summary>
        ''' Decrypts data using the provided private key
        ''' </summary>
        Public Function Decrypt(ByVal encryptedData As Data, ByVal PrivateKey As PrivateKey) As Data
            _rsa.ImportParameters(PrivateKey.ToParameters)
            Return DecryptPrivate(encryptedData)
        End Function

        ''' <summary>
        ''' Decrypts data using the provided private key as XML
        ''' </summary>
        Public Function Decrypt(ByVal encryptedData As Data, ByVal PrivateKeyXML As String) As Data
            LoadKeyXml(PrivateKeyXML, True)
            Return DecryptPrivate(encryptedData)
        End Function

        Private Sub LoadKeyXml(ByVal keyXml As String, ByVal isPrivate As Boolean)
            Try
                _rsa.FromXmlString(keyXml)
            Catch ex As Security.XmlSyntaxException
                Dim s As String
                If isPrivate Then
                    s = "private"
                Else
                    s = "public"
                End If
                Throw New Security.XmlSyntaxException( _
                    String.Format("The provided {0} encryption key XML does not appear to be valid.", s), ex)
            End Try
        End Sub

        Private Function DecryptPrivate(ByVal encryptedData As Data) As Data
            Return New Data(_rsa.Decrypt(encryptedData.Bytes, False))
        End Function

        ''' <summary>
        ''' gets the default RSA provider using the specified key size; 
        ''' note that Microsoft's CryptoAPI has an underlying file system dependency that is unavoidable
        ''' </summary>
        ''' <remarks>
        ''' http://support.microsoft.com/default.aspx?scid=http://support.microsoft.com:80/support/kb/articles/q322/3/71.asp&amp;NoWebContent=1
        ''' </remarks>
        Private Function GetRSAProvider() As RSACryptoServiceProvider
            Dim rsa As RSACryptoServiceProvider = Nothing
            Dim csp As CspParameters = Nothing
            Try
                csp = New CspParameters
                csp.KeyContainerName = _KeyContainerName
                rsa = New RSACryptoServiceProvider(_KeySize, csp)
                rsa.PersistKeyInCsp = False
                RSACryptoServiceProvider.UseMachineKeyStore = True
                Return rsa
            Catch ex As System.Security.Cryptography.CryptographicException
                If ex.Message.ToLower.IndexOf("csp for this implementation could not be acquired") > -1 Then
                    Throw New Exception("Unable to obtain Cryptographic Service Provider. " & _
                        "Either the permissions are incorrect on the " & _
                        "'C:\Documents and Settings\All Users\Application Data\Microsoft\Crypto\RSA\MachineKeys' " & _
                        "folder, or the current security context '" & Security.Principal.WindowsIdentity.GetCurrent.Name & "'" & _
                        " does not have access to this folder.", ex)
                Else
                    Throw
                End If
            Finally
                If Not rsa Is Nothing Then
                    rsa = Nothing
                End If
                If Not csp Is Nothing Then
                    csp = Nothing
                End If
            End Try
        End Function

    End Class

#End Region

#Region "  Data"

    ''' <summary>
    ''' represents Hex, Byte, Base64, or String data to encrypt/decrypt;
    ''' use the .Text property to set/get a string representation 
    ''' use the .Hex property to set/get a string-based Hexadecimal representation 
    ''' use the .Base64 to set/get a string-based Base64 representation 
    ''' </summary>
    Public Class Data
        Private _b As Byte()
        Private _MaxBytes As Integer = 0
        Private _MinBytes As Integer = 0
        Private _StepBytes As Integer = 0

        ''' <summary>
        ''' Determines the default text encoding across ALL Data instances
        ''' </summary>
        Public Shared DefaultEncoding As System.Text.Encoding = System.Text.Encoding.GetEncoding("Windows-1252")

        ''' <summary>
        ''' Determines the default text encoding for this Data instance
        ''' </summary>
        Public Encoding As System.Text.Encoding = DefaultEncoding

        ''' <summary>
        ''' Creates new, empty encryption data
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' Creates new encryption data with the specified byte array
        ''' </summary>
        Public Sub New(ByVal b As Byte())
            _b = b
        End Sub

        ''' <summary>
        ''' Creates new encryption data with the specified string; 
        ''' will be converted to byte array using default encoding
        ''' </summary>
        Public Sub New(ByVal s As String)
            Me.Text = s
        End Sub

        ''' <summary>
        ''' Creates new encryption data using the specified string and the 
        ''' specified encoding to convert the string to a byte array.
        ''' </summary>
        Public Sub New(ByVal s As String, ByVal encoding As System.Text.Encoding)
            Me.Encoding = encoding
            Me.Text = s
        End Sub

        ''' <summary>
        ''' returns true if no data is present
        ''' </summary>
        Public ReadOnly Property IsEmpty() As Boolean
            Get
                If _b Is Nothing Then
                    Return True
                End If
                If _b.Length = 0 Then
                    Return True
                End If
                Return False
            End Get
        End Property

        ''' <summary>
        ''' allowed step interval, in bytes, for this data; if 0, no limit
        ''' </summary>
        Public Property StepBytes() As Integer
            Get
                Return _StepBytes
            End Get
            Set(ByVal Value As Integer)
                _StepBytes = Value
            End Set
        End Property

        ''' <summary>
        ''' allowed step interval, in bits, for this data; if 0, no limit
        ''' </summary>
        Public Property StepBits() As Integer
            Get
                Return _StepBytes * 8
            End Get
            Set(ByVal Value As Integer)
                _StepBytes = Value \ 8
            End Set
        End Property

        ''' <summary>
        ''' minimum number of bytes allowed for this data; if 0, no limit
        ''' </summary>
        Public Property MinBytes() As Integer
            Get
                Return _MinBytes
            End Get
            Set(ByVal Value As Integer)
                _MinBytes = Value
            End Set
        End Property

        ''' <summary>
        ''' minimum number of bits allowed for this data; if 0, no limit
        ''' </summary>
        Public Property MinBits() As Integer
            Get
                Return _MinBytes * 8
            End Get
            Set(ByVal Value As Integer)
                _MinBytes = Value \ 8
            End Set
        End Property

        ''' <summary>
        ''' maximum number of bytes allowed for this data; if 0, no limit
        ''' </summary>
        Public Property MaxBytes() As Integer
            Get
                Return _MaxBytes
            End Get
            Set(ByVal Value As Integer)
                _MaxBytes = Value
            End Set
        End Property

        ''' <summary>
        ''' maximum number of bits allowed for this data; if 0, no limit
        ''' </summary>
        Public Property MaxBits() As Integer
            Get
                Return _MaxBytes * 8
            End Get
            Set(ByVal Value As Integer)
                _MaxBytes = Value \ 8
            End Set
        End Property

        ''' <summary>
        ''' Returns the byte representation of the data; 
        ''' This will be padded to MinBytes and trimmed to MaxBytes as necessary!
        ''' </summary>
        Public Property Bytes() As Byte()
            Get
                If _MaxBytes > 0 Then
                    If _b.Length > _MaxBytes Then
                        Dim b(_MaxBytes - 1) As Byte
                        Array.Copy(_b, b, b.Length)
                        _b = b
                    End If
                End If
                If _MinBytes > 0 Then
                    If _b.Length < _MinBytes Then
                        Dim b(_MinBytes - 1) As Byte
                        Array.Copy(_b, b, _b.Length)
                        _b = b
                    End If
                End If
                Return _b
            End Get
            Set(ByVal Value As Byte())
                _b = Value
            End Set
        End Property

        ''' <summary>
        ''' Sets or returns text representation of bytes using the default text encoding
        ''' </summary>
        Public Property Text() As String
            Get
                If _b Is Nothing Then
                    Return ""
                Else
                    '-- need to handle nulls here; oddly, C# will happily convert
                    '-- nulls into the string whereas VB stops converting at the
                    '-- first null!
                    Dim i As Integer = Array.IndexOf(_b, CType(0, Byte))
                    If i >= 0 Then
                        Return Me.Encoding.GetString(_b, 0, i)
                    Else
                        Return Me.Encoding.GetString(_b)
                    End If
                End If
            End Get
            Set(ByVal Value As String)
                _b = Me.Encoding.GetBytes(Value)
            End Set
        End Property

        ''' <summary>
        ''' Sets or returns Hex string representation of this data
        ''' </summary>
        Public Property Hex() As String
            Get
                Return Utils.ToHex(_b)
            End Get
            Set(ByVal Value As String)
                _b = Utils.FromHex(Value)
            End Set
        End Property

        ''' <summary>
        ''' Sets or returns Base64 string representation of this data
        ''' </summary>
        Public Property Base64() As String
            Get
                Return Utils.ToBase64(_b)
            End Get
            Set(ByVal Value As String)
                _b = Utils.FromBase64(Value)
            End Set
        End Property

        ''' <summary>
        ''' Returns text representation of bytes using the default text encoding
        ''' </summary>
        Public Shadows Function ToString() As String
            Return Me.Text
        End Function

        ''' <summary>
        ''' returns Base64 string representation of this data
        ''' </summary>
        Public Function ToBase64() As String
            Return Me.Base64
        End Function

        ''' <summary>
        ''' returns Hex string representation of this data
        ''' </summary>
        Public Function ToHex() As String
            Return Me.Hex
        End Function

    End Class

#End Region

#Region "  Utils"

    ''' <summary>
    ''' Friend class for shared utility methods used by multiple Encryption classes
    ''' </summary>
    Friend Class Utils

        ''' <summary>
        ''' converts an array of bytes to a string Hex representation
        ''' </summary>
        Friend Shared Function ToHex(ByVal ba() As Byte) As String
            If ba Is Nothing OrElse ba.Length = 0 Then
                Return ""
            End If
            Const HexFormat As String = "{0:X2}"
            Dim sb As New StringBuilder
            For Each b As Byte In ba
                sb.Append(String.Format(HexFormat, b))
            Next
            Return sb.ToString
        End Function

        ''' <summary>
        ''' converts from a string Hex representation to an array of bytes
        ''' </summary>
        Friend Shared Function FromHex(ByVal hexEncoded As String) As Byte()
            If hexEncoded Is Nothing OrElse hexEncoded.Length = 0 Then
                Return Nothing
            End If
            Try
                Dim l As Integer = Convert.ToInt32(hexEncoded.Length / 2)
                Dim b(l - 1) As Byte
                For i As Integer = 0 To l - 1
                    b(i) = Convert.ToByte(hexEncoded.Substring(i * 2, 2), 16)
                Next
                Return b
            Catch ex As Exception
                Throw New System.FormatException("The provided string does not appear to be Hex encoded:" & _
                    Environment.NewLine & hexEncoded & Environment.NewLine, ex)
            End Try
        End Function

        ''' <summary>
        ''' converts from a string Base64 representation to an array of bytes
        ''' </summary>
        Friend Shared Function FromBase64(ByVal base64Encoded As String) As Byte()
            If base64Encoded Is Nothing OrElse base64Encoded.Length = 0 Then
                Return Nothing
            End If
            Try
                Return Convert.FromBase64String(base64Encoded)
            Catch ex As System.FormatException
                Throw New System.FormatException("The provided string does not appear to be Base64 encoded:" & _
                    Environment.NewLine & base64Encoded & Environment.NewLine, ex)
            End Try
        End Function

        ''' <summary>
        ''' converts from an array of bytes to a string Base64 representation
        ''' </summary>
        Friend Shared Function ToBase64(ByVal b() As Byte) As String
            If b Is Nothing OrElse b.Length = 0 Then
                Return ""
            End If
            Return Convert.ToBase64String(b)
        End Function

        ''' <summary>
        ''' retrieve an element from an XML string
        ''' </summary>
        Friend Shared Function GetXmlElement(ByVal xml As String, ByVal element As String) As String
            Dim m As Match
            m = Regex.Match(xml, "<" & element & ">(?<Element>[^>]*)</" & element & ">", RegexOptions.IgnoreCase)
            If m Is Nothing Then
                Throw New Exception("Could not find <" & element & "></" & element & "> in provided Public Key XML.")
            End If
            Return m.Groups("Element").ToString
        End Function

        ''' <summary>
        ''' Returns the specified string value from the application .config file
        ''' </summary>
        Friend Shared Function GetConfigString(ByVal key As String, _
            Optional ByVal isRequired As Boolean = True) As String

            Dim s As String = CType(ConfigurationManager.AppSettings.Get(key), String)
            If s = Nothing Then
                If isRequired Then
                    Throw New ConfigurationErrorsException("key <" & key & "> is missing from .config file")
                Else
                    Return ""
                End If
            Else
                Return s
            End If
        End Function

        Friend Shared Function WriteConfigKey(ByVal key As String, ByVal value As String) As String
            Dim s As String = "<add key=""{0}"" value=""{1}"" />" & Environment.NewLine
            Return String.Format(s, key, value)
        End Function

        Friend Shared Function WriteXmlElement(ByVal element As String, ByVal value As String) As String
            Dim s As String = "<{0}>{1}</{0}>" & Environment.NewLine
            Return String.Format(s, element, value)
        End Function

        Friend Shared Function WriteXmlNode(ByVal element As String, Optional ByVal isClosing As Boolean = False) As String
            Dim s As String
            If isClosing Then
                s = "</{0}>" & Environment.NewLine
            Else
                s = "<{0}>" & Environment.NewLine
            End If
            Return String.Format(s, element)
        End Function

    End Class

#End Region

    End Namespace
