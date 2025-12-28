import pandas as pd
import osmnx as ox
import networkx as nx
import folium
import numpy as np
from shapely.geometry import Point, MultiPoint


HEDEF_MAHALLELER = ['Konak Mh.', 'Beşevler Mh.', 'Üçevler Mh.']
CSV_FILE = 'konteyner_lokasyonlari_tipli.csv'

ARAC_SAYISI = 3
ARAC_KAPASITESI = 8000

ox.settings.timeout = 600
ox.settings.log_console = False
ox.settings.use_cache = True


print("Veriler okunuyor...")
df = pd.read_csv(CSV_FILE)

df = df[df['Mahalle'].isin(HEDEF_MAHALLELER)].copy()
df = df.dropna(subset=['Enlem', 'Boylam'])

if df.empty:
    raise ValueError("Seçilen mahallelerde konteynır yok!")

np.random.seed(42)
df['doluluk_orani'] = np.random.randint(30, 100, size=len(df))
df['tahmini_yuk'] = df['doluluk_orani'] / 100 * 120
df['priority'] = df['doluluk_orani']


print("Konteynır noktalarından alan oluşturuluyor...")

points = [Point(lon, lat) for lat, lon in zip(df['Enlem'], df['Boylam'])]
multi_point = MultiPoint(points)

polygon = multi_point.convex_hull.buffer(0.0015)


print("Yol ağı indiriliyor (polygon bazlı, hızlı)...")

G = ox.graph_from_polygon(
    polygon,
    network_type="drive",
    simplify=True
)

print(f"Yol ağı hazır → {len(G.nodes)} node")

df = df.sort_values('priority', ascending=False).reset_index(drop=True)

vehicles = [
    {'id': i, 'capacity_left': ARAC_KAPASITESI, 'containers': []}
    for i in range(ARAC_SAYISI)
]

for _, row in df.iterrows():
    for v in vehicles:
        if v['capacity_left'] >= row['tahmini_yuk']:
            v['containers'].append(row)
            v['capacity_left'] -= row['tahmini_yuk']
            break

def nearest_neighbor(points):
    unvisited = list(range(len(points)))
    current = 0
    path = [current]
    unvisited.remove(current)

    while unvisited:
        d = np.linalg.norm(points[unvisited] - points[current], axis=1)
        nxt = unvisited[np.argmin(d)]
        path.append(nxt)
        unvisited.remove(nxt)
        current = nxt

    return path

m = folium.Map(
    location=[df['Enlem'].mean(), df['Boylam'].mean()],
    zoom_start=14,
    tiles="cartodbpositron"
)

colors = ['red', 'blue', 'green', 'purple']

for v in vehicles:
    if len(v['containers']) < 2:
        continue

    v_df = pd.DataFrame(v['containers']).reset_index(drop=True)
    order = nearest_neighbor(v_df[['Enlem', 'Boylam']].values)
    v_df = v_df.iloc[order]

    nodes = ox.nearest_nodes(G, v_df['Boylam'], v_df['Enlem'])
    color = colors[v['id'] % len(colors)]

    for i in range(len(nodes) - 1):
        try:
            route = nx.shortest_path(G, nodes[i], nodes[i+1], weight='length')
            coords = [(G.nodes[n]['y'], G.nodes[n]['x']) for n in route]
            folium.PolyLine(coords, color=color, weight=4).add_to(m)
        except:
            pass

    for _, r in v_df.iterrows():
        folium.CircleMarker(
            [r['Enlem'], r['Boylam']],
            radius=4,
            color=color,
            fill=True,
            popup=f"Araç {v['id']} | %{int(r['doluluk_orani'])}"
        ).add_to(m)

output_file = "alternatif_rota.html"
m.save(output_file)

print(f"\nBİTTİ ✅ → {output_file}")
