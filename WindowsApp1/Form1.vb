Imports System.IO
Imports System.Net
Imports System.Threading
Imports System.Management
Imports System.Security

Public Class Form1

    Dim login As Integer
    Dim confirm As Integer
    Dim runlogin As Integer
    Dim updateneeded As Integer
    Dim firstlaunch As Integer
    Dim confirmhwid As Integer
    Dim updatecolor As Integer

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        WebBrowser1.Navigate("http://artificialaim.net/forums/member.php?action=login")
        WebBrowser2.Navigate("http://artificialaim.net/loader/usercheck.php")
        WebBrowser3.Navigate("http://artificialaim.net/loader/hwid.php")

        'Loads user settings
        If My.Settings.check = True Then
            TextBox1.Text = My.Settings.username
            TextBox2.Text = My.Settings.password
            CheckBox1.Checked = My.Settings.check
        Else
            'Do nothing
        End If

        'Check connection to website
        Try
            My.Computer.Network.Ping("www.yoursite.com")
        Catch
            MsgBox("Error: Unable to connect to the website", vbCritical)
            Application.Exit()
        End Try

        'Generate HWID
        Dim hw As New clsComputerInfo

        Dim cpu As String
        Dim mb As String
        Dim mac As String
        Dim hwid As String

        cpu = hw.GetProcessorId()
        mb = hw.GetMotherboardID()
        mac = hw.GetMACAddress()
        hwid = cpu + mb + mac

        Dim hwidEncrypted As String = Strings.UCase(hw.getMD5Hash(cpu & mb & mac))

        txtHWID.Text = hwidEncrypted
        'HWID Generated

        Timer1.Start()
        Timer2.Start()

        'Local Version
        Dim currentVerison As String
        currentVerison = "1.0.0.0"

        'Web Version
        Dim address As String = "http://artificialaim.net/loader/version.txt"
        Dim client As WebClient = New WebClient()
        Dim reader As StreamReader = New StreamReader(client.OpenRead(address))
        Label4.Text = reader.ReadToEnd
        Label4.ForeColor = Color.Transparent
        Label4.Location = New Point(1000, 2000)

        If (currentVerison < Label4.Text) Then
            updateneeded = 1
        ElseIf (currentVerison > Label4.Text) Then
            updateneeded = 1
        Else
            updateneeded = 0
        End If

        Label6.Text = "Current Status: Waiting for input"

        If My.Computer.FileSystem.FileExists("C:\temp\Loader\Artificial Aim.Hook") Then
        Else
            firstlaunch = 1
            System.IO.Directory.CreateDirectory("C:\temp\Loader\")
            My.Computer.FileSystem.WriteAllText("c:\temp\Loader\Artificial Aim.Hook", "", False)
            'Dim ToHideDir As New System.IO.DirectoryInfo("C:\temp\Loader\")
            'ToHideDir.Attributes = IO.FileAttributes.Hidden
        End If


        'Dim pName As String = "Steam"
        'Dim psList() As Process
        'Try
        'psList = Process.GetProcesses()
        'For Each p As Process In psList
        'If (pName = p.ProcessName) Then
        'MsgBox("Steam is currently running! Please close steam before using the loader", MsgBoxStyle.Exclamation)
        'Me.Close()
        'End If
        'Next p

        'Catch ex As Exception
        'MsgBox(ex.Message)
        'End Try

        'Dim appPath As String = My.Application.Info.DirectoryPath
        'If appPath.Contains("C:\") Then
        'MsgBox("The loader must be run from a USB! Please move the loader's .exe onto a USB then execute!")
        'Me.Close()
        'End If
    End Sub
    ' Confirm HWID
    Private Class clsComputerInfo
        'Get processor ID
        Friend Function GetProcessorId() As String
            Dim strProcessorID As String = String.Empty
            Dim query As New SelectQuery("Win32_processor")
            Dim search As New ManagementObjectSearcher(query)
            Dim info As ManagementObject
            For Each info In search.Get()
                strProcessorID = info("processorID").ToString()
            Next
            Return strProcessorID
        End Function
        ' Get MAC Address
        Friend Function GetMACAddress() As String
            Dim mc As ManagementClass = New ManagementClass("Win32_NetworkAdapterConfiguration")
            Dim moc As ManagementObjectCollection = mc.GetInstances()
            Dim MacAddress As String = String.Empty
            For Each mo As ManagementObject In moc
                If (MacAddress.Equals(String.Empty)) Then
                    If CBool(mo("IPEnabled")) Then MacAddress = mo("MacAddress").ToString()
                    mo.Dispose()
                End If
                MacAddress = MacAddress.Replace(":", String.Empty)
            Next
            Return MacAddress
        End Function
        ' Get Motherboard ID
        Friend Function GetMotherboardID() As String
            Dim strMotherboardID As String = String.Empty
            Dim query As New SelectQuery("Win32_BaseBoard")
            Dim search As New ManagementObjectSearcher(query)
            Dim info As ManagementObject
            For Each info In search.Get()
                strMotherboardID = info("product").ToString()
            Next
            Return strMotherboardID
        End Function
        ' Encrypt HWID
        Friend Function getMD5Hash(ByVal strToHash As String) As String
            Dim md5Obj As New System.Security.Cryptography.MD5CryptoServiceProvider
            Dim bytesToHash() As Byte = System.Text.Encoding.ASCII.GetBytes(strToHash)
            bytesToHash = md5Obj.ComputeHash(bytesToHash)
            Dim strResult As String = ""
            For Each b As Byte In bytesToHash
                strResult += b.ToString("x2")
            Next
            Return strResult
        End Function
    End Class
    ' Timer start
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If (firstlaunch = 1) Then
            Form6.Show()
            Timer1.Stop()
        Else
            Timer1.Stop()
        End If
    End Sub
    ' Login button
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Button1.Enabled = False
        login = 1

        Try
            WebBrowser1.Document.GetElementById("username").SetAttribute("value", TextBox1.Text)
            WebBrowser1.Document.GetElementById("password").SetAttribute("value", TextBox2.Text)
            WebBrowser1.Document.GetElementById("submit").InvokeMember("click")
        Catch
            MsgBox("Error: Login function not setup right. Yell at the 'dev' until he figures out how to read the README.md", vbCritical)
        End Try

        Try
            WebBrowser2.Document.GetElementById("username").SetAttribute("value", TextBox1.Text)
            WebBrowser2.Document.GetElementById("submit").InvokeMember("click")
        Catch
            MsgBox("Error: Usercheck function not setup right. Yell at the 'dev' until he figures out how to read the README.md", vbCritical)
        End Try

        Try
            WebBrowser3.Document.GetElementById("username").SetAttribute("value", TextBox1.Text)
            WebBrowser3.Document.GetElementById("hwidin").SetAttribute("value", txtHWID.Text)
            WebBrowser3.Document.GetElementById("submit").InvokeMember("click")
        Catch
            MsgBox("Error: HWID function not setup right. Yell at the 'dev' until he figures out how to read the README.md", vbCritical)
        End Try
    End Sub

    ' Exit button
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Application.Exit()
    End Sub
    ' Login check
    Private Sub WebBrowser1_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WebBrowser1.DocumentCompleted
        If login = 1 Then
            login = 0
            Try
                Label6.Text = "Current Status: Checking password"
                If WebBrowser1.DocumentText.Contains("<p>You have successfully been logged in") Then
                    Label6.Text = "Current Status: Password accepted"
                    confirm = 1
                Else
                    Label6.ForeColor = Color.Red
                    Label6.Text = "Current Status: Password rejected!"
                    WebBrowser1.Navigate("http://artificialaim.net/forums/member.php?action=login")
                    MsgBox("Error: Username or password is incorrect", vbCritical)
                    Thread.Sleep(250)
                    Button1.Enabled = True
                End If
            Catch ex As Exception
                MsgBox("Error: Forum login not working. Yell at the 'dev' until they learn to read the README.md", vbCritical)
            End Try
        End If
    End Sub
    ' Status check
    Private Sub WebBrowser2_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WebBrowser1.DocumentCompleted
        If confirm = 1 Then
            confirm = 0
            Try
                Label6.Text = "Current Status: Checking account status"

                If WebBrowser2.DocumentText.Contains("2") Then ' Check if they are "registered"
                    Label6.Text = "Current Status: Group accepted"
                    confirmhwid = 1
                Else
                    If WebBrowser2.DocumentText.Contains("7") Then ' Check if they are "banned"
                        Label6.Text = "Current Status: User is banned"
                        Label6.ForeColor = Color.Red
                        confirmhwid = 0
                    Else
                        If WebBrowser2.DocumentText.Contains("5") Then ' Checking if their accaount is valid"
                            Label6.Text = "Confirm Email Address"
                            Label6.ForeColor = Color.Red
                            confirmhwid = 0
                        Else
                            If WebBrowser2.DocumentText.Contains("8") Then ' Check if they are "Premium CS:GO"
                                Label6.Text = "VIP"
                                confirmhwid = 1
                            Else
                                If WebBrowser2.DocumentText.Contains("10") Then ' Check if they are "Premium CS:GO Beta"
                                    Label6.Text = "Current Status: Group accepted"
                                    confirmhwid = 1
                                Else
                                    If WebBrowser2.DocumentText.Contains("9") Then ' Check if they are "Premium CS:GO Lite"
                                        Label6.Text = "Current Status: Group accepted"
                                        confirmhwid = 1
                                    Else
                                        If WebBrowser2.DocumentText.Contains("11") Then ' Check if they are "Premium Garry's Mod"
                                            Label6.Text = "Current Status: Group accepted"
                                            confirmhwid = 1
                                        Else
                                            If WebBrowser2.DocumentText.Contains("4") Then ' Check if they are "Admin"
                                                Label6.Text = "Current Status: Group accepted"
                                                confirmhwid = 1
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                MsgBox("Error: usercheck_get.php not working. Yell at the 'dev' until they learn to read the README.md", vbCritical)
            End Try
        End If

        'Admin---------------------------------4
        'Premium Garry's Mod------------------11
        'Moderator----------------------------10
        'Developer-----------------------------9
        'VIP-----------------------------------8
        'Banned--------------------------------7
        'Registered User-----------------------2

    End Sub
    ' HWID check
    Private Sub WebBrowser3_DocumentCompleted_1(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WebBrowser3.DocumentCompleted
        If confirmhwid = 1 Then
            confirmhwid = 0
            Try
                Label6.Text = "Current Status: Checking HWID"
                If WebBrowser3.DocumentText.Contains("HWID is correct") Then
                    Label6.Text = "Current Status: HWID accepted!"

                    If (CheckBox1.Checked = True) Then
                        My.Settings.check = True
                    ElseIf (CheckBox1.Checked = False) Then
                        My.Settings.check = False
                    End If

                    My.Settings.username = TextBox1.Text
                    My.Settings.password = TextBox2.Text
                    My.Settings.Save()
                    TextBox1.Text = ""
                    TextBox2.Text = ""
                    TextBox1.Focus()
                    Form3.Show()
                    Me.Hide()
                    Button1.Enabled = True
                ElseIf WebBrowser3.DocumentText.Contains("HWID is not correct") Then
                    WebBrowser1.Navigate("http://artificialaim.net/forums/member.php?action=login")
                    WebBrowser2.Navigate("http://artificialaim.net/loader/usercheck.php")
                    WebBrowser3.Navigate("http://artificialaim.net/loader/hwid.php")
                    Label6.ForeColor = Color.Red
                    Label6.Text = "Current Status: HWID rejected!"
                    MsgBox("Error: HWID is incorrect", vbCritical)
                    Thread.Sleep(250)
                    Button1.Enabled = True
                ElseIf WebBrowser3.DocumentText.Contains("") Then
                    WebBrowser1.Navigate("http://artificialaim.net/forums/member.php?action=login")
                    WebBrowser2.Navigate("http://artificialaim.net/loader/usercheck.php")
                    WebBrowser3.Navigate("http://artificialaim.net/loader/hwid.php")
                    Label6.ForeColor = Color.Red
                    Label6.Text = "Current Status: HWID rejected!"
                    MsgBox("Error: No user with that name (HWID)", vbCritical)
                    Thread.Sleep(250)
                    Button1.Enabled = True
                End If
            Catch ex As Exception
                MsgBox("Error: hwid_get.php not working. Yell at the 'dev' until they learn to read the README.md", vbCritical)
            End Try
        End If
    End Sub
    ' Login fix
    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click
        Me.Hide()
        Form2.Show()
    End Sub
    ' Update Script

    Private Sub Button3_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged

    End Sub

    Private Sub Label6_Click(sender As Object, e As EventArgs) Handles Label6.Click

    End Sub

    'This is the updater script. Un-comment it if you want the update function. It's kind of useless in my opinion UNLESS you are doing more than just CS:GO cheats.
    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        If (updateneeded = 1) Then
            Timer2.Stop()
            Dim exePath As String = Application.ExecutablePath()
            My.Computer.FileSystem.WriteAllText("C:\temp\Loader\Artificial Aim.Hook.Path", exePath, False)
            Me.Hide()
            Dim appPath As String = My.Application.Info.DirectoryPath
            If My.Computer.FileSystem.FileExists(appPath + "/Updater.exe") Then
                Process.Start(appPath + "/Updater.exe")
                Application.Exit()
            Else
                My.Computer.Network.DownloadFile("http://artificialaim.net/loader/Updater.exe", appPath + "/Updater.exe")
                Process.Start(appPath + "/Updater.exe")
                Application.Exit()
            End If
        Else
            Timer2.Stop()
        End If
    End Sub
End Class

'-----------------------------------------------------
' Coded by /id/Thaisen! Free loader source
' https://github.com/ThaisenPM/Cheat-Loader-CSGO
' Note to the person using this, removing this
' text is in violation of the license you agreed
' to by downloading. Only you can see this so what
' does it matter anyways.
' Copyright © ThaisenPM 2017
' Licensed under a MIT license
' Read the terms of the license here
' https://github.com/ThaisenPM/Cheat-Loader-CSGO/blob/master/LICENSE
'-----------------------------------------------------
