import pandas as pd
import folium

# Veriyi oku
df = pd.read_csv('mahalle_bazli_cizgi_rotalar.csv')

# Haritayı Bursa Nilüfer merkezli başlat
m = folium.Map(location=[40.21, 28.93], zoom_start=13, tiles='cartodbpositron')

# Mahalleleri farklı renklerle çiz
colors = ['red', 'blue', 'green', 'purple', 'orange', 'darkred', 'lightred', 'beige', 'darkblue', 'darkgreen', 'cadetblue', 'darkpurple', 'white', 'pink', 'lightblue', 'lightgreen', 'gray', 'black', 'lightgray']
mahalle_listesi = df['Mahalle'].unique()
mahalle_renk_sozlugu = {mahalle: colors[i % len(colors)] for i, mahalle in enumerate(mahalle_listesi)}

for _, row in df.iterrows():
    color = mahalle_renk_sozlugu.get(row['Mahalle'], 'blue')
    
    # Başlangıç ve bitiş noktalarını birleştir
    folium.PolyLine(
        locations=[[row['Start_Lat'], row['Start_Lon']], [row['End_Lat'], row['End_Lon']]],
        color=color,
        weight=3,
        opacity=0.7,
        tooltip=f"Mahalle: {row['Mahalle']} | Sıra: {row['Sira']}"
    ).add_to(m)

# Haritayı kaydet
m.save("guzergah_inceleme.html")
print("Harita oluşturuldu: 'guzergah_inceleme.html' dosyasını tarayıcınla açabilirsin.")