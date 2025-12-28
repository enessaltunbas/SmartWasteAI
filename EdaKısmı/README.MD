🚛 Nilüfer Belediyesi Akıllı Atık Yönetimi ve Rota Optimizasyonu
Bu proje, Nilüfer Belediyesi sınırlarındaki çöp konteynırı lokasyonlarını GPS verilerinden tespit etmek, bu noktaları mahalle bazlı optimize etmek ve araçlar için en uygun toplama güzergahlarını harita üzerinde görselleştirmek amacıyla geliştirilmiştir.

🚀 Proje Aşamaları
Sistem üç ana modülden oluşmaktadır:

1. Veri Ön İşleme ve Temizleme (clean_data.py)
Ham GPS verilerini (all_merged_data*.csv) işleyerek konteynır lokasyonlarını filtreler.

Kriter: 30 saniye ile 10 dakika arası duraklamalar "konteynır boşaltma" olarak kabul edilir.

Tekilleştirme: Aynı koordinattaki mükerrer kayıtlar temizlenir.

Eşleştirme: fleet.csv üzerinden araç tipleri (Crane Truck, Garbage Truck vb.) veriye eklenir.

2. Mahalle Bazlı Rota Optimizasyonu (optimize_route.py)
Tespit edilen noktaları mahalle bazında gruplandırır ve Nearest Neighbor (En Yakın Komşu) algoritmasını kullanarak sıralar.

Araçların mahalle içinde gereksiz "git-gel" yapmasını önler.

Kepler.gl ve diğer görselleştirme araçları için başlangıç-bitiş koordinatlarını içeren çizgi (line) verisi üretir.

3. İnteraktif Harita Görselleştirme (visualize_map.py)
Hesaplanan rotaları folium kütüphanesi kullanarak interaktif bir HTML haritasına dönüştürür.

Her mahalle farklı bir renkle gösterilir.

Güzergahlar gerçekçi çizgi katmanları olarak eklenir.

Durak sırası ve araç tipi bilgilerini içeren "tooltip"ler eklenmiştir.

🛠 Kurulum ve Gereksinimler
Projenin çalışması için Python 3.x gereklidir. Gerekli kütüphaneleri yüklemek için:

Bash

pip install pandas numpy folium
📂 Dosya Yapısı ve Kullanım
Veri Hazırlama: Ham verilerinizi proje klasörüne koyun ve temizleme scriptini çalıştırın.

Girdi: all_merged_data*.csv, fleet.csv

Çıktı: konteyner_lokasyonlari_tipli.csv

Optimizasyon: Temizlenmiş veri üzerinden rotaları oluşturun.

Çıktı: mahalle_bazli_noktalar.csv, mahalle_bazli_cizgi_rotalar.csv

Görselleştirme: Haritayı oluşturun.

Çıktı: guzergah_inceleme.html

📊 Örnek Çıktı Görünümü
Oluşturulan guzergah_inceleme.html dosyasını herhangi bir tarayıcıda açarak;

Mahalle mahalle ayrılmış toplama yollarını,

Konteynırların toplama sırasını,

Araç tiplerine göre dağılımı inceleyebilirsiniz.

Not: Bu proje Nilüfer Hackathon 2025 kapsamında geliştirilmektedir.

Geliştiriciler: Ravan Novruzov, Enes Altunbaş, Oğuzhan Hekimoğlu