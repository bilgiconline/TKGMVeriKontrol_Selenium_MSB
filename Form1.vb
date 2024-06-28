Imports OpenQA.Selenium
Imports OpenQA.Selenium.Support.UI
Imports OpenQA.Selenium.Keys
Imports OpenQA.Selenium.Chrome
Imports System.Threading.Thread
Imports System.Data.Odbc
Imports System.Text.RegularExpressions
Imports OpenQA.Selenium.Interactions
Imports System.IO
Imports System.Threading
Imports System.ComponentModel
Imports System.Text.Json
Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Public Class Form1
    Dim SiteURL As String
    Dim q_CityName As String
    Dim q_DistrictName As String
    Dim q_NeighName As String
    Dim q_ParcelNo As String
    Dim q_AdaNo As String
    Friend Shared driverService As ChromeDriverService
    Friend Shared driver As IWebDriver
    Friend Shared dbList As New List(Of String)
    Friend Shared landCsv As New List(Of String)
    Dim c_cities As New List(Of String)
    Dim c_district As New List(Of String)
    Dim c_neighList As New List(Of String)
    Dim featureList As New List(Of Feature)
    Dim exportFeaturesList As New List(Of String)
    Dim step1 As Boolean = False
    Dim step2 As Boolean = False

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        driverService = ChromeDriverService.CreateDefaultService()
        driverService.HideCommandPromptWindow = True
        '  driver = New ChromeDriver(driverService, New ChromeOptions())
        driver = New ChromeDriver("chromedriver.exe")
        '  driver = WebDriver.Chrome()

    End Sub
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        SiteURL = "https://cbsapi.tkgm.gov.tr/megsiswebapi.v3/api/parsel/124842/10710/77/"
        driver.Navigate().GoToUrl(SiteURL)
        Try
            exportJson()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim provinces As String = "Erzurum|Trabzon|Gümüşhane|Artvin|Ağrı|Ardahan|Bayburt|Erzincan|Rize|Iğdır|Kars"
        Dim prov() As String = provinces.Split("|")
        For u = 0 To UBound(prov)
            Dim citySelector = driver.FindElement(By.Id("province-select"))
            citySelector.Click()
            citySelector.SendKeys(prov(u))
            citySelector.Click()
            Thread.Sleep(1000)

            Dim element3 = driver.FindElements(By.Id("district-select"))
            Thread.Sleep(1000)
            For i = 0 To element3.Count - 1
                Dim districtSelector3 = driver.FindElement(By.Id("district-select"))
                Dim parcala As String() = element3(0).Text.ToString.Split(vbCrLf)
                For t = 1 To UBound(parcala)
                    Debug.Print(parcala(t))
                    districtSelector3.SendKeys(parcala(t))
                    Thread.Sleep(1000)
                    Dim neighSelector = driver.FindElement(By.Id("neighborhood-select"))
                    Thread.Sleep(1000)
                    Dim SelectElement = New SelectElement(neighSelector)
                    For Each aListText In SelectElement.Options
                        Using WriterStream As StreamWriter = New StreamWriter("D:\MahSelect.txt", True)
                            WriterStream.WriteLine(prov(u) & "|" & parcala(t) & "|" & aListText.GetAttribute("value").ToString & "|" & aListText.Text)
                            'Debug.Print(aListText.GetAttribute("value").ToString & "|" & aListText.Text)
                            WriterStream.Close()
                            WriterStream.Dispose()
                        End Using
                    Next
                Next
            Next
        Next
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        For i = 0 To 1
            Dim citySelector = driver.FindElement(By.Id("province-select"))
            citySelector.Click()
            citySelector.SendKeys("Erzurum")
            citySelector.Click()
            citySelector.Click()
            citySelector.SendKeys("Erzurum")
            citySelector.Click()
            Try
                Dim districtSelector = driver.FindElement(By.Id("district-select"))

                districtSelector.Click()
                districtSelector.SendKeys("Yakutiye")
                districtSelector.Click()
                districtSelector.SendKeys("Yakutiye")
                districtSelector.Click()


                Dim neighSelector = driver.FindElement(By.Id("neighborhood-select"))

                neighSelector.Click()
                neighSelector.SendKeys("Dumlu")
                neighSelector.Click()
                neighSelector.SendKeys("Dumlu")
                neighSelector.Click()
                Dim blockText = driver.FindElement(By.Id("block-input"))
                '  blockText.SendKeys("")
                blockText.SendKeys("10710")
                'blockText.SendKeys("10710")
                Dim parcelText = driver.FindElement(By.Id("parcel-input"))
                parcelText.SendKeys("76")
                'parcelText.SendKeys("76")
            Catch ex As Exception
                '   MsgBox(ex.ToString)
            End Try
        Next
    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckForIllegalCrossThreadCalls = False
        SiteURL = "https://parselsorgu.tkgm.gov.tr/#ara/idari/124842/10710/76/"
        driver.Navigate().GoToUrl(SiteURL)
        Timer1.Start()
        Dim strRead() As String = Split(StreamReaderProgram(Application.StartupPath & "\BlgList.txt"), vbCrLf)
        For k = 0 To UBound(strRead)
            Dim line As String = strRead(k).Replace(vbCrLf, "").Replace(Chr(10), "").Replace(Chr(13), "").Replace(vbLf, "")
            Debug.Print(line)
            dbList.Add(line)
            If c_cities.Contains(line.Split(",")(0)) = False Then
                c_cities.Add(line.Split(",")(0))
            End If
            If c_district.Contains(line.Split(",")(1)) = False Then
                c_district.Add(line.Split(",")(1))
            End If
            If c_neighList.Contains(line.Split(",")(2)) = False Then
                c_neighList.Add(line.Split(",")(2))
            End If
        Next
    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Try
            Dim btn = driver.FindElement(By.Id("terms-ok"))
            btn.Click()
            Timer1.Stop()
        Catch ex As Exception
            '   MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If BackgroundWorker1.IsBusy = False Then
            BackgroundWorker1.RunWorkerAsync()
        Else
            MsgBox("Zaten okuma başlamış durumda. Bitmesini bekleyin.")
        End If
    End Sub

    Private Sub Form1_Closed(sender As Object, e As EventArgs) Handles MyBase.Closed
        driver.Quit()
        driver.Dispose()
    End Sub

    'Private Sub btnStop_Click(sender As Object, e As EventArgs) Handles btnStop.Click
    '    btnStop.Text = "Stopping service..."
    '    btnStop.Enabled = False
    '    driver.Quit()
    '    driver.Dispose()
    '    btnStop.Text = "Done."
    'End Sub


    'Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
    '    Dim path As String
    '    path = SiteURL.Text
    '    driver.Navigate().GoToUrl(path)
    'End Sub

    'Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
    '    Dim exportPath As String = ExportNotePath.Text
    '    Using aStrWriter As StreamWriter = New StreamWriter(exportPath, False)
    '        aStrWriter.WriteLine(String.Join(vbCrLf, GetPageList))
    '        aStrWriter.Close()
    '        aStrWriter.Dispose()
    '    End Using
    'End Sub

    'Private Sub Form1_Closed(sender As Object, e As EventArgs) Handles MyBase.Closed
    '    driver.Quit()
    '    driver.Dispose()
    'End Sub

    'Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
    '    Try
    '        Using aStrReader As StreamReader = New StreamReader(ReadTextTb.Text)
    '            While Not aStrReader.EndOfStream
    '                Dim activeLine As String = aStrReader.ReadLine
    '                If InStr(activeLine, "https:") > 0 And InStr(activeLine, "|") > 0 Then
    '                    activeLine = activeLine.Split("|")(0)
    '                    If InStr(StreamReaderProgram(WriteTb.Text), activeLine) = 0 Then
    '                        Dim result As String = NavigatePageandGetInfo(activeLine)
    '                        If result = "Error." Then
    '                            MsgBox("Please check internet connection or site connect.")
    '                            aStrReader.Close()
    '                            aStrReader.Dispose()
    '                            Exit While
    '                        Else
    '                            StreamWriterProgram(WriteTb.Text, result)
    '                        End If
    '                    End If
    '                End If
    '            End While
    '            aStrReader.Close()
    '            aStrReader.Dispose()
    '        End Using
    '    Catch ex As Exception
    '        Debug.Print(ex.ToString)
    '        MsgBox("Check read site file. Was wrong.")
    '        Exit Sub
    '    End Try
    'End Sub
    Function StreamWriterProgram(Path As String, StringofLine As String) As String
        CheckDirectoriesandCreate(Path)
        If File.Exists(Path) = False Then
            File.Create(Path).Close()
            File.CreateText(Path).Dispose()
        End If
        Dim enc As Text.Encoding
        enc = System.Text.Encoding.GetEncoding("Windows-1254")
        Try
            Using WriterStream As StreamWriter = New StreamWriter(Path, True, System.Text.Encoding.GetEncoding("Windows-1254"))
                WriterStream.WriteLine(ToolboxConvertTurkishCh(StringofLine.ToString))
                WriterStream.Close()
                WriterStream.Dispose()
            End Using
        Catch ex As Exception
            Debug.Print(ex.ToString)
            MsgBox("Write path was wrong. Check file path.")
        End Try
        Return StreamWriterProgram
    End Function
    Sub CheckDirectoriesandCreate(PathDir As String)
        Dim i
        Dim tmpStringList() As String
        Dim LocalorConnectionPath As String
        '-nas-3bm
        Dim tmpStr() As String
        tmpStr = PathDir.Split("\")
        If Mid(PathDir, 1, 2) = "\\" Then
            LocalorConnectionPath = "\\" & tmpStr(2) & "\" 'Mid(PathDir, 1, InStr(PathDir.Replace("\\", ""), "\") + 1) & "\" & tmpStr(3)
        ElseIf Mid(PathDir, 2, 2) = ":\" Then
            LocalorConnectionPath = tmpStr(0) & "\"
        Else
            'MsgBox("Disk tespit edilemedi.")
            Exit Sub
        End If
        If PathDir.EndsWith("\") Then PathDir = Mid(PathDir, 1, InStrRev(PathDir, "\") - 1)
        tmpStringList = PathDir.Replace(LocalorConnectionPath, "").Split("\")
        Dim tmpCumulativestring As String
        For i = 0 To tmpStringList.Count - 1
            If InStr(tmpStringList(i).ToString, ".") = 0 Then
                If i = 0 Then
                    tmpCumulativestring = LocalorConnectionPath & tmpStringList(i).ToString
                    If Directory.Exists(tmpCumulativestring) = False Then
                        My.Computer.FileSystem.CreateDirectory(tmpCumulativestring)
                    End If
                Else
                    tmpCumulativestring = tmpCumulativestring & "\" & tmpStringList(i).ToString
                    If Directory.Exists(tmpCumulativestring) = False Then
                        My.Computer.FileSystem.CreateDirectory(tmpCumulativestring)
                        '   "ż"
                    End If
                End If
            End If
        Next
    End Sub
    Function StreamReaderProgram(Path As String) As String
        Dim Start As Boolean = False
        Try
            If File.Exists(Path) = True Then
                Dim ReadStream As Stream = New FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite) 'New Stream(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                Using StreamReaderGetInfos As StreamReader = New StreamReader(ReadStream) 'FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    While Not StreamReaderGetInfos.EndOfStream
                        Dim aReadLine As String
                        aReadLine = StreamReaderGetInfos.ReadLine
                        If Start = False Then
                            StreamReaderProgram = ToolboxConvertTurkishCh(aReadLine)
                            Start = True
                        Else
                            StreamReaderProgram = StreamReaderProgram & vbCrLf & ToolboxConvertTurkishCh(aReadLine)
                        End If
                    End While
                    StreamReaderGetInfos.Close()
                    StreamReaderGetInfos.Dispose()
                End Using
                If StreamReaderProgram = "" Then
                    StreamReaderProgram = ""
                End If
            Else
                StreamReaderProgram = ""
            End If
        Catch ex As Exception
            StreamReaderProgram = ""
        End Try
        Return StreamReaderProgram
    End Function
    Function ToolboxConvertTurkishCh(originalWord As String) As String
        If Not originalWord = "" Then
            ToolboxConvertTurkishCh = originalWord
            ToolboxConvertTurkishCh = ToolboxConvertTurkishCh.Replace("ï»¿", "")
            ToolboxConvertTurkishCh = ToolboxConvertTurkishCh.Replace("Ã‡", "Ç")
            ToolboxConvertTurkishCh = ToolboxConvertTurkishCh.Replace("Ã§", "ç")
            ToolboxConvertTurkishCh = ToolboxConvertTurkishCh.Replace("Ãœ", "Ü")
            ToolboxConvertTurkishCh = ToolboxConvertTurkishCh.Replace("Ã¼", "ü")
            ToolboxConvertTurkishCh = ToolboxConvertTurkishCh.Replace("Å", "Ş")
            ToolboxConvertTurkishCh = ToolboxConvertTurkishCh.Replace("ÅŸ", "ş")
            ToolboxConvertTurkishCh = ToolboxConvertTurkishCh.Replace("Ä°", "İ")
            ToolboxConvertTurkishCh = ToolboxConvertTurkishCh.Replace("Ä±", "ı")
            ToolboxConvertTurkishCh = ToolboxConvertTurkishCh.Replace("Ã–", "Ö")
            ToolboxConvertTurkishCh = ToolboxConvertTurkishCh.Replace("Ã¶", "ö")
            ToolboxConvertTurkishCh = ToolboxConvertTurkishCh.Replace("ï»¿", "")
            ToolboxConvertTurkishCh = ToolboxConvertTurkishCh.Replace("ÄŸ", "ğ")
            ToolboxConvertTurkishCh = ToolboxConvertTurkishCh.Replace("Ä", "Ğ")
        Else
            ToolboxConvertTurkishCh = ""
        End If
        Return ToolboxConvertTurkishCh
    End Function

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim element3 = driver.FindElements(By.Id("district-select"))
        For i = 0 To element3.Count - 1
            Debug.Print(element3(i).Text.ToString)
        Next
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        If step1 = True Then
            If BackgroundWorker2.IsBusy = False Then
                BackgroundWorker2.RunWorkerAsync()
            Else
                MsgBox("Zaten kontrol işlemi başlamış durumda. Bitmesini bekleyin.")
            End If
        Else
            MsgBox("Uygun csv dosyası yüklenmediği için kontrole başlanmadı.")
        End If
    End Sub

    Sub exportFeatures(feature As Feature)
        Dim exportline As String = ""
        If CheckBox1.Checked = True Then
            exportline = exportline & "," & feature.properties.ilAd
        End If
        If CheckBox2.Checked = True Then
            exportline = exportline & "," & feature.properties.ilceAd
        End If
        If CheckBox3.Checked = True Then
            exportline = exportline & "," & feature.properties.mahalleAd
        End If
        If CheckBox4.Checked = True Then
            exportline = exportline & "," & feature.properties.mevkii
        End If
        If CheckBox5.Checked = True Then
            exportline = exportline & "," & feature.properties.alan.Replace(".", "").Replace(",", ".")
        End If
        If CheckBox6.Checked = True Then
            exportline = exportline & "," & feature.properties.adaNo
        End If
        If CheckBox7.Checked = True Then
            exportline = exportline & "," & feature.properties.parselNo
        End If
        If CheckBox8.Checked = True Then
            exportline = exportline & "," & feature.properties.nitelik
        End If
        If CheckBox9.Checked = True Then
            exportline = exportline & "," & feature.properties.pafta
        End If
        If CheckBox10.Checked = True Then
            exportline = exportline & "," & feature.properties.durum
        End If
        exportline = exportline & "," & "https://cbsapi.tkgm.gov.tr/megsiswebapi.v3/api/parsel/" & feature.properties.mahalleId & "/" & feature.properties.adaNo & "/" & feature.properties.parselNo & "/" & "," & "https://cbsapi.tkgm.gov.tr/megsiswebapi.v3/api/parsel/" & feature.properties.mahalleId & "/" & feature.properties.adaNo & "/" & feature.properties.parselNo & "/"
        exportFeaturesList.Add(exportline)
    End Sub

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Try
            If File.Exists(TextBox1.Text) = True Then
                landCsv.Clear()
                Dim strRead() As String = Split(StreamReaderProgram(TextBox1.Text), vbCrLf)
                For k = 2 To UBound(strRead)
                    Dim line As String = strRead(k).Replace(vbCrLf, "").Replace(Chr(10), "").Replace(Chr(13), "").Replace(vbLf, "")
                    Dim lineRead() As String = Split(line, ("|"))
                    Try
                        landCsv.Add(lineRead(3) & "," & lineRead(4) & "," & lineRead(20) & "," & lineRead(6) & "," & lineRead(7) & "," & lineRead(8) & "," & lineRead(9) & "," & lineRead(19) & "," & k + 1)
                        Label1.Text = "Okunan Veri: " & CStr(k + 1)
                    Catch ex As Exception
                        RichTextBox1.Text = RichTextBox1.Text & vbCrLf & "Hata: Satır: " & k + 1 & " - Veri okumasında hata vardı."
                    End Try
                    RichTextBox1.Text = RichTextBox1.Text & vbCrLf & k + 1 & " adet veri okunmuştur."
                Next
            Else
                RichTextBox1.Text = RichTextBox1.Text & vbCrLf & "Hata: " & " - Dosya bulunamadı."
            End If
        Catch ex As Exception
            RichTextBox1.Text = RichTextBox1.Text & vbCrLf & "Hata: " & " - Bilinmeyen hata."
        End Try
    End Sub

    Private Sub BackgroundWorker2_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker2.DoWork
        For k = 0 To landCsv.Count - 1
            Dim jsonString As String
            If c_cities.Contains(landCsv(k).Split(",")(0)) = False Then
                RichTextBox2.Text = RichTextBox2.Text & vbCrLf & "Satır: " & landCsv(k).Split(",")(8) & " " & landCsv(k).Split(",")(0) & " şehir ismi bulunamadı."
                GoTo aNext
            End If
            If c_district.Contains(landCsv(k).Split(",")(1)) = False Then
                RichTextBox2.Text = RichTextBox2.Text & vbCrLf & "Satır: " & landCsv(k).Split(",")(8) & " " & landCsv(k).Split(",")(1) & " ilçe ismi bulunamadı."
                GoTo aNext
            End If
            If c_neighList.Contains(landCsv(k).Split(",")(2)) = False Then
                RichTextBox2.Text = RichTextBox2.Text & vbCrLf & "Satır: " & landCsv(k).Split(",")(8) & " " & landCsv(k).Split(",")(2) & " mahalle ismi bulunamadı."
                GoTo aNext
            End If
            Try
                jsonString = QueryEvent(landCsv(k).Split(",")(0), landCsv(k).Split(",")(1), landCsv(k).Split(",")(2), landCsv(k).Split(",")(3), landCsv(k).Split(",")(4))
                jsonString = Mid(jsonString, InStr(jsonString, "{"))
                jsonString = Mid(jsonString, 1, InStrRev(jsonString, "}"))
                If Not jsonString = "" Then
                    Try
                        Dim feature As Feature = ParseJson(jsonString)
                        featureList.Add(feature)
                        ' exportFeatures(feature)
                    Catch ex As Exception
                        RichTextBox2.Text = RichTextBox2.Text & vbCrLf & "Satır: " & landCsv(k).Split(",")(8) & " verisi okunamadı."
                    End Try
                Else
                    RichTextBox2.Text = RichTextBox2.Text & vbCrLf & "Satır: " & landCsv(k).Split(",")(8) & " veriye ulaşılmadı."
                End If
            Catch ex As Exception
                RichTextBox2.Text = RichTextBox2.Text & vbCrLf & "Satır: " & landCsv(k).Split(",")(8) & " verisi okunamadı."
            End Try
            Label3.Text = k + 1 & "/" & landCsv.Count
aNext:
        Next
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        step1 = True
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        denn()
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        If step2 = True Then
            If BackgroundWorker3.IsBusy = False Then
                BackgroundWorker3.RunWorkerAsync()
            End If
        Else
            MsgBox("Kontrol tamamlanmadı veya yapılmadı.")
        End If
    End Sub

    Private Sub BackgroundWorker3_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorker3.DoWork
        exportFeaturesList.Clear()

        For k = 0 To featureList.Count - 1
            exportFeatures(featureList(k))
        Next
        CheckDirectoriesandCreate(TextBox2.Text)
        Try
            Try
                My.Computer.FileSystem.DeleteFile(TextBox2.Text)
            Catch ex As Exception

            End Try
            Using WriterStream As StreamWriter = New StreamWriter(TextBox2.Text, True)
                For k = 0 To exportFeaturesList.Count - 1
                    WriterStream.WriteLine(Mid(exportFeaturesList(k), 2))
                Next
                WriterStream.Close()
                WriterStream.Dispose()
            End Using
        Catch ex As Exception
            Debug.Print(ex.ToString)
            MsgBox("Yazdırılacak dosya yolu bulunamadı.")
        End Try
    End Sub

    Private Sub BackgroundWorker2_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BackgroundWorker2.RunWorkerCompleted
        step2 = True
    End Sub

End Class

