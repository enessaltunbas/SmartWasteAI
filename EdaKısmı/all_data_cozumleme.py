import pandas as pd
import io
import glob

# süreyi saniyeye çevireceğiz
def hms_to_sec(t):
    t = str(t).strip()
    if not t or t == 'nan': return 0
    days = 0
    if 'g' in t: 
        parts = t.split('g')
        try:
            days = int(parts[0].strip())
            t = parts[1].strip()
        except: pass
    parts = t.split(':')
    try:
        if len(parts) == 3:
            return days*86400 + int(parts[0])*3600 + int(parts[1])*60 + int(parts[2])
        elif len(parts) == 2:
            return days*86400 + int(parts[0])*60 + int(parts[1])
    except: return 0
    return 0

# dosya temizleme adımları
dosyalar = sorted(glob.glob("all_merged_data*.csv"))
print(f"Tespit edilen parçalar: {dosyalar}")

sutun_isimleri = ['#', 'vehicle_id', 'Enlem', 'Boylam', 'Duraklama Süresi', 'Rölanti Süresi', 
                  'Yükseklik', 'Durum', 'Açıklama', 'Tarih', 'Saat', 'Gun', 
                  'Hız(km/sa)', 'Mesafe(km)', 'Mesafe Sayacı(km)', 'Adres', 'Mahalle', 'Kaynak']

parca_listesi = []

for dosya in dosyalar:
    print(f"{dosya} işleniyor...")
    temiz_satirlar = []
    with open(dosya, 'r', encoding='utf-8-sig') as f:
        for line in f:
            line = line.strip()
            if line.startswith('"') and line.endswith('"'):
                line = line[1:-1]
            line = line.replace('""', '"')
            
            if line.startswith('#') or not line or 'vehicle_id' in line:
                continue
            temiz_satirlar.append(line)
    
    df_parca = pd.read_csv(io.StringIO("\n".join(temiz_satirlar)), header=None, names=sutun_isimleri)
    parca_listesi.append(df_parca)


df = pd.concat(parca_listesi, ignore_index=True)

# 3. konteynır noktaları filtreleme 
df['Saniye'] = df['Duraklama Süresi'].apply(hms_to_sec)


konteyner_filtresi = df[(df['Saniye'] >= 30) & (df['Saniye'] <= 600)].copy()

konteyner_filtresi['Lat_rnd'] = konteyner_filtresi['Enlem'].round(4)
konteyner_filtresi['Lon_rnd'] = konteyner_filtresi['Boylam'].round(4)

son_liste = konteyner_filtresi.drop_duplicates(subset=['Lat_rnd', 'Lon_rnd']).copy()

son_liste = son_liste.drop(columns=['Saniye', 'Lat_rnd', 'Lon_rnd'])

print(f"Filtreleme tamamlandı: {len(son_liste)} benzersiz nokta tespit edildi.")

# fleet.csv ile birlestirme
print("Araç tipleri fleet.csv dosyasından eşleştiriliyor...")
try:
    fleet_df = pd.read_csv('fleet.csv')
    fleet_mapping = fleet_df[['vehicle_id', 'vehicle_type']].drop_duplicates()
    

    final_df = pd.merge(son_liste, fleet_mapping, on='vehicle_id', how='left')
    

    cikti_dosyasi = 'konteyner_lokasyonlari_tipli.csv'
    final_df.to_csv(cikti_dosyasi, index=False, encoding='utf-8-sig')
    
    print("-" * 30)
    print(f"İŞLEM BAŞARIYLA TAMAMLANDI!")
    print(f"Final dosyası: '{cikti_dosyasi}'")
    print(f"Toplam Nokta Sayısı: {len(final_df)}")
    print(f"Toplam Sütun Sayısı: {len(final_df.columns)}")
    print("-" * 30)

except FileNotFoundError:
    print("HATA: 'fleet.csv' dosyası bulunamadı. Lütfen dosyanın script ile aynı klasörde olduğundan emin olun.")
except Exception as e:
    print(f"Bir hata oluştu: {e}")