Imports System.IO
Imports System.Text.Json
Imports System.Threading
Imports OpenQA.Selenium
Imports OpenQA.Selenium.Support.UI
Imports System.Text.RegularExpressions
Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Module WebApp
    Dim driveMore As IWebDriver = Form1.driver
    Public Function ParseJson(jsonString As String) As Feature
        Dim feature As New Feature()
        feature.type = ExtractJsonValue(jsonString, "type")

        Dim geometryJson As String = ExtractJsonObject(jsonString, "geometry")
        Dim propertiesJson As String = ExtractJsonObject(jsonString, "properties")

        feature.geometry = ParseGeometry(geometryJson)
        feature.properties = ParseProperties(propertiesJson)

        Return feature
    End Function

    Private Function ExtractJsonValue(jsonString As String, key As String) As String
        Dim pattern As String = $"""{key}"":\s*""([^""]*)"""
        Dim match As Match = Regex.Match(jsonString, pattern)
        Return If(match.Success, match.Groups(1).Value, String.Empty)
    End Function

    Private Function ExtractJsonObject(jsonString As String, key As String) As String
        Dim pattern As String = $"""{key}"":\s*(\{{.*?\}})"
        Dim match As Match = Regex.Match(jsonString, pattern)
        Return If(match.Success, match.Groups(1).Value, String.Empty)
    End Function

    Private Function ParseGeometry(jsonString As String) As Geometry
        Dim geometry As New Geometry()
        geometry.type = ExtractJsonValue(jsonString, "type")

        Dim coordinatesJson As String = ExtractJsonArray(jsonString, "coordinates")
        geometry.coordinates = ParseCoordinates(coordinatesJson)

        Return geometry
    End Function

    Private Function ParseCoordinates(jsonString As String) As List(Of List(Of List(Of Double)))
        Dim coordinates As New List(Of List(Of List(Of Double)))()

        ' Remove outer brackets
        jsonString = jsonString.Trim("["c, "]"c)

        ' Split the string into individual coordinate sets
        Dim coordinateSets As String() = jsonString.Split(New String() {"]], ["}, StringSplitOptions.None)

        For Each dset As String In coordinateSets
            Dim innerCoordinates As New List(Of List(Of Double))()

            ' Split the coordinate set into individual coordinates
            Dim coordinatePairs As String() = dset.Split(New String() {"], ["}, StringSplitOptions.None)

            For Each pair As String In coordinatePairs
                Dim coords As New List(Of Double)()
                Dim values As String() = pair.Split(","c)

                Dim lat As Double
                Dim lon As Double

                If Double.TryParse(values(0).Trim("["c, "]"c), lat) AndAlso Double.TryParse(values(1).Trim("["c, "]"c), lon) Then
                    coords.Add(lat)
                    coords.Add(lon)
                End If

                innerCoordinates.Add(coords)
            Next

            coordinates.Add(innerCoordinates)
        Next

        Return coordinates
    End Function

    Private Function ExtractJsonArray(jsonString As String, key As String) As String
        Dim pattern As String = $"""{key}"":\s*(\[\[.*?\]\])"
        Dim match As Match = Regex.Match(jsonString, pattern)
        Return If(match.Success, match.Groups(1).Value, String.Empty)
    End Function

    Private Function ParseProperties(jsonString As String) As Properties
        Dim props As New Properties()
        props.ilceAd = ExtractJsonValue(jsonString, "ilceAd")
        props.mevkii = ExtractJsonValue(jsonString, "mevkii")

        Dim ilId As Integer
        If Integer.TryParse(ExtractJsonValue(jsonString, "ilId"), ilId) Then
            props.ilId = ilId
        End If

        props.durum = ExtractJsonValue(jsonString, "durum")

        Dim ilceId As Integer
        If Integer.TryParse(ExtractJsonValue(jsonString, "ilceId"), ilceId) Then
            props.ilceId = ilceId
        End If

        props.zeminKmdurum = ExtractJsonValue(jsonString, "zeminKmdurum")
        props.parselNo = ExtractJsonValue(jsonString, "parselNo")
        props.mahalleAd = ExtractJsonValue(jsonString, "mahalleAd")
        props.ozet = ExtractJsonValue(jsonString, "ozet")
        props.gittigiParselListe = ExtractJsonValue(jsonString, "gittigiParselListe")
        props.gittigiParselSebep = ExtractJsonValue(jsonString, "gittigiParselSebep")
        props.alan = ExtractJsonValue(jsonString, "alan")
        props.adaNo = ExtractJsonValue(jsonString, "adaNo")
        props.nitelik = ExtractJsonValue(jsonString, "nitelik")
        props.ilAd = ExtractJsonValue(jsonString, "ilAd")

        Dim mahalleId As Integer
        If Integer.TryParse(ExtractJsonValue(jsonString, "mahalleId"), mahalleId) Then
            props.mahalleId = mahalleId
        End If

        props.pafta = ExtractJsonValue(jsonString, "pafta")

        Return props
    End Function
    Sub denn()
        Dim json As String = File.ReadAllText("C:\Users\bilgi\OneDrive\Masaüstü\1.txt")
        Try


            ' JSON verisini JObject olarak yükle
            ' Ana feature'ı seç
            Dim anaFeature As JObject = JObject.Parse(json)("properties")

        ' "\" karakterlerine göre JSON verisini parçala
        Dim features As New List(Of JObject)
        Dim startIndex As Integer = 0
        Dim currentIndex As Integer = 0

        While currentIndex < json.Length
            If json(currentIndex) = "\"c Then
                ' "\" karakteri bulunduğunda, parçayı al ve yeni bir JObject olarak ekle
                Dim featureJson As String = json.Substring(startIndex, currentIndex - startIndex + 1)
                features.Add(JObject.Parse(featureJson))
                startIndex = currentIndex + 1 ' Bir sonraki parçanın başlangıç indeksini güncelle
            End If
            currentIndex += 1
        End While

        ' Ana feature ile diğer feature'ları birleştir
        Dim allFeatures As New List(Of JObject)
        allFeatures.Add(anaFeature)
        allFeatures.AddRange(features)

            ' Toplam feature sayısını kontrol et
            If allFeatures.Count = 5 Then
                Console.WriteLine("Toplam 5 farklı feature var.")
            Else
                Console.WriteLine("Toplam feature sayısı uygun değil.")
            End If
        Catch ex As Exception
            Debug.WriteLine(ex.ToString)
        End Try
    End Sub
    Sub exportJson()
        Try
            Dim jsonString As String
            jsonString = QueryEvent("Erzurum", "Yakutiye", "Dumlu", "10710", "7").Replace("<html><head><meta name=color-scheme content=light dark><meta charset=utf-8></head><body><pre>", "").Replace("</pre><div class=json-formatter-container></div></body></html>", "")
            jsonString = Mid(jsonString, InStr(jsonString, "{"))
            jsonString = Mid(jsonString, 1, InStrRev(jsonString, "}"))
            Debug.Print(jsonString)
            If Not jsonString = "" Then
                '  Dim feature As Feature = ParseJson(jsonString)

                Dim feature As Feature = ParseJson(jsonString)

                ' Deserialize edilmiş veriyi kullanabilirsiniz
                Console.WriteLine("İlçe Adı: " & feature.properties.ilceAd)
                Console.WriteLine("Mevkii: " & feature.properties.mevkii)
            Else
                Debug.WriteLine("Veri bulunamadı veya boş.")
            End If

        Catch ex As Exception
            Debug.WriteLine("HATA:" & vbCrLf & ex.ToString & vbCrLf & "Hata--" & vbCrLf)
        End Try
    End Sub
    Function QueryEvent(q_CityName As String, q_DistrictName As String, q_NeighName As String, q_AdaNo As String, q_ParcelNo As String) As String
        Try
            Dim found As Boolean = False
            Dim fullLine As String = q_CityName & "," & q_DistrictName & "," & q_NeighName & ","
            For i = 0 To Form1.dbList.Count - 1
                If Form1.dbList(i).Contains(fullLine) = True Then
                    Dim getLinkAdress As String = "https://cbsapi.tkgm.gov.tr/megsiswebapi.v3/api/parsel/" & Form1.dbList(i).Split(",")(3).ToString & "/" & q_AdaNo & "/" & q_ParcelNo & "/"
                    driveMore.Navigate().GoToUrl(getLinkAdress)
                    found = True
                    Exit For
                End If
            Next
            If found = False Then
                QueryEvent = ""
            Else
                Dim queryanswer As String = driveMore.PageSource.ToString '.Replace("\", "").Replace(Chr(34), "").Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "")
                If InStr(queryanswer, "This XML file does not appear") > 0 Then
                    QueryEvent = ""
                Else
                    QueryEvent = queryanswer.Replace("<html><head><meta name=color-scheme content=light dark><meta charset=utf-8></head><body><pre>", "").Replace("</pre><div class=json-formatter-container></div></body></html>", "")
                End If
            End If
        Catch ex As Exception
            QueryEvent = ""
        End Try
    End Function
    Sub QueryEvent1(q_CityName As String, q_DistrictName As String, q_NeighName As String, q_AdaNo As String, q_ParcelNo As String)
        '<select class="form-control flat" name="province-select" id="province-select"><option value="-3" selected="selected">İl Seçiniz</option><option value="23">Adana</option><option value="24">Adıyaman</option><option value="25">Afyonkarahisar</option><option value="26">Ağrı</option><option value="90">Aksaray</option><option value="27">Amasya</option><option value="28">Ankara</option><option value="29">Antalya</option><option value="97">Ardahan</option><option value="30">Artvin</option><option value="31">Aydın</option><option value="32">Balıkesir</option><option value="96">Bartın</option><option value="94">Batman</option><option value="91">Bayburt</option><option value="33">Bilecik</option><option value="34">Bingöl</option><option value="35">Bitlis</option><option value="36">Bolu</option><option value="37">Burdur</option><option value="38">Bursa</option><option value="39">Çanakkale</option><option value="40">Çankırı</option><option value="41">Çorum</option><option value="42">Denizli</option><option value="43">Diyarbakır</option><option value="103">Düzce</option><option value="44">Edirne</option><option value="45">Elazığ</option><option value="46">Erzincan</option><option value="47">Erzurum</option><option value="48">Eskişehir</option><option value="49">Gaziantep</option><option value="50">Giresun</option><option value="51">Gümüşhane</option><option value="52">Hakkari</option><option value="53">Hatay</option><option value="98">Iğdır</option><option value="54">Isparta</option><option value="56">İstanbul</option><option value="57">İzmir</option><option value="68">Kahramanmaraş</option><option value="100">Karabük</option><option value="92">Karaman</option><option value="58">Kars</option><option value="59">Kastamonu</option><option value="60">Kayseri</option><option value="101">Kilis</option><option value="93">Kırıkkale</option><option value="61">Kırklareli</option><option value="62">Kırşehir</option><option value="63">Kocaeli</option><option value="64">Konya</option><option value="65">Kütahya</option><option value="66">Malatya</option><option value="67">Manisa</option><option value="69">Mardin</option><option value="55">Mersin</option><option value="70">Muğla</option><option value="71">Muş</option><option value="72">Nevşehir</option><option value="73">Niğde</option><option value="74">Ordu</option><option value="102">Osmaniye</option><option value="75">Rize</option><option value="76">Sakarya</option><option value="77">Samsun</option><option value="85">Şanlıurfa</option><option value="78">Siirt</option><option value="79">Sinop</option><option value="95">Şırnak</option><option value="80">Sivas</option><option value="81">Tekirdağ</option><option value="82">Tokat</option><option value="83">Trabzon</option><option value="84">Tunceli</option><option value="86">Uşak</option><option value="87">Van</option><option value="99">Yalova</option><option value="88">Yozgat</option><option value="89">Zonguldak</option></select>
        Dim citySelector = driveMore.FindElement(By.Id("province-select"))
        citySelector.Click()
        citySelector.SendKeys(q_CityName)
        citySelector.Click()
        Thread.Sleep(1000)
        Dim districtSelector = driveMore.FindElement(By.Id("district-select"))
        Dim element3 = driveMore.FindElements(By.Id("district-select"))
        Try
            Dim splitValues As String() = element3(0).Text.ToString.Split(vbCrLf)
            Dim found_District As Boolean = False
            For t = 1 To UBound(splitValues)
                If splitValues(t).ToString = q_DistrictName Then
                    found_District = True
                    districtSelector.SendKeys(splitValues(t))
                    Exit For
                End If
            Next
            If found_District = False Then
                'Hata = İlçe Bulunmadı
                Exit Sub
            Else
                found_District = False
                Thread.Sleep(1000)
                Dim neighSelector = driveMore.FindElement(By.Id("neighborhood-select"))
                Thread.Sleep(1000)
                Dim SelectElement = New SelectElement(neighSelector)
                For Each aListText In SelectElement.Options
                    If aListText.Text = q_NeighName Then
                        found_District = True
                        Exit For
                    End If
                Next
                If found_District = False Then
                    'Hata = Mah Bulunmadı
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            'Hata = İl Bulunmadı
        End Try
    End Sub
End Module


Public Class Feature
    Public Property type As String
    Public Property geometry As Geometry
    Public Property properties As Properties
End Class

Public Class Geometry
    Public Property type As String
    Public Property coordinates As List(Of List(Of List(Of Double)))
End Class

Public Class Properties
    Public Property ilceAd As String
    Public Property mevkii As String
    Public Property ilId As Integer
    Public Property durum As String
    Public Property ilceId As Integer
    Public Property zeminKmdurum As String
    Public Property parselNo As String
    Public Property mahalleAd As String
    Public Property ozet As String
    Public Property gittigiParselListe As String
    Public Property gittigiParselSebep As String
    Public Property alan As String
    Public Property adaNo As String
    Public Property nitelik As String
    Public Property ilAd As String
    Public Property mahalleId As String
    Public Property pafta As String
End Class

