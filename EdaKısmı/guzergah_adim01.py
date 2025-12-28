import pandas as pd
import numpy as np

df = pd.read_csv('konteyner_lokasyonlari_tipli.csv')

def optimize_route(points):
    """Noktaları birbirine en yakın olana göre sıraya dizer (Nearest Neighbor)."""
    if len(points) <= 1:
        return list(range(len(points)))
    
    unvisited = list(range(len(points)))
    current = 0 # İlk noktadan başla
    path = [current]
    unvisited.remove(current)
    
    while unvisited:
        current_coord = points[current]
        remaining_coords = points[unvisited]
        # Mesafeleri hesapla (Karesini almak hız için yeterlidir)
        dists = np.sum((remaining_coords - current_coord)**2, axis=1)
        nearest_idx = np.argmin(dists)
        current = unvisited[nearest_idx]
        path.append(current)
        unvisited.remove(current)
    return path

# 2. Mahalle bazlı gruplayıp optimize et
optimized_list = []
line_data = []

for mahalle, group in df.groupby('Mahalle'):
    if group.empty: continue
    
    group = group.reset_index(drop=True)
    order = optimize_route(group[['Enlem', 'Boylam']].values)
    ordered_group = group.iloc[order].copy()
    ordered_group['Sira'] = range(1, len(ordered_group) + 1)
    optimized_list.append(ordered_group)
    
    # Çizgi (Line) verisi oluştur (Kepler'de güzergahı görmek için)
    for i in range(len(ordered_group) - 1):
        line_data.append({
            'Mahalle': mahalle,
            'Start_Lat': ordered_group.iloc[i]['Enlem'],
            'Start_Lon': ordered_group.iloc[i]['Boylam'],
            'End_Lat': ordered_group.iloc[i+1]['Enlem'],
            'End_Lon': ordered_group.iloc[i+1]['Boylam'],
            'Sira': i + 1,
            'Vehicle_Type': ordered_group.iloc[i]['vehicle_type']
        })

# 3. Sonuçları kaydet
# Dosya A: Sıralı noktalar
pd.concat(optimized_list).to_csv('mahalle_bazli_noktalar.csv', index=False)
# Dosya B: Çizgiler (Güzergah)
pd.DataFrame(line_data).to_csv('mahalle_bazli_cizgi_rotalar.csv', index=False)

print("İşlem bitti! İki dosya oluşturuldu.")