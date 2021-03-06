﻿using System.Collections.Generic;
using UnityEngine;

namespace XYZMap
{

    class TileVector : MapTile
    {

        private List<TilePOI> pois = new List<TilePOI>();
        private List<TileExtrusion> extrusions = new List<TileExtrusion>();
        private List<TileFlat> flats = new List<TileFlat>();
        private List<TileLine> lines = new List<TileLine>();

        public TileVector( Map map, string quadKey)
        {
            this.map = map;
            this.quadKey = quadKey;
            initFromQuadKey(quadKey);
            url = getMapUrl(map.parent.vectorTileUrl);
        }

        public override void Update( bool active )
        {
            if (!loaded) return;

            foreach (TileLine line in lines)
            {
                line.Update(active);
            }

            return;
            foreach (TilePOI poi in pois)
            {
                poi.Update(active);
            }
            foreach (TileExtrusion extrusion in extrusions)
            {
                extrusion.Update(active);
            }
            foreach (TileFlat flat in flats)
            {
                flat.Update(active);
            }
        }

        public override void onDataLoaded(WWW www)
        {

            if (gameObject == null)
            {
                gameObject = new GameObject();
                gameObject.transform.parent = map.parent.tiles.transform;
            }

            JSONObject obj = new JSONObject(www.text);
            
            //buildings
            if (map.parent.colorBuildings.a != 0)
            {
                extrusions.Add(new TileExtrusion(this, obj["buildings"]["features"], map.parent.tiles, map.parent.colorBuildings));
            }

            //landuse
            if (map.parent.colorLandUse.a != 0)
            {
                flats.Add(new TileFlat(this, obj["landuse"]["features"], map.parent.tiles, map.parent.colorLandUse, -0.5F));
            }

            //earth 
            if (map.parent.colorEarth.a != 0)
            {
                flats.Add(new TileFlat(this, obj["earth"]["features"], map.parent.tiles, map.parent.colorEarth, -1));
            }

            //water
            if (map.parent.colorWater.a != 0)
            {
                flats.Add(new TileFlat(this, obj["water"]["features"], map.parent.tiles, map.parent.colorWater, -2));
            }



            //roads types:
            //highway, major_road, minor_road, path, aeroway, rail, ferry, piste, aerialway, racetrack, portage_way

            float scale = 1 / map.resolution(map.zoom);
            if (map.parent.colorRoads.a != 0)
            {
                List<string> roads = new List<string>(new string[] { "highway", "major_road", "minor_road" });
                TileLine tl = new TileLine(this, obj["roads"]["features"], map.parent.tiles, map.parent.colorRoads, roads, map.parent.roadWidth * scale);
                lines.Add(tl);
            }

            if (map.parent.colorRails.a != 0)
            {
                List<string> rails = new List<string>(new string[] { "rail" });
                TileLine tl = new TileLine(this, obj["roads"]["features"], map.parent.tiles, map.parent.colorRails, rails, map.parent.railWidth * scale);
                lines.Add(tl);
                tl.gameObject.transform.position = new Vector3(0, .25f, 0);
            }

            loaded = true;

            /*
            color = new Color( 0,1,0 );
            List<string> rails = new List<string>(new string[] { "rail" });
            lines.Add(new TileLine(this, obj["roads"]["features"], map.parent.tiles, color, rails, 4 * scale));
            
            color = new Color( 0,0,1 );
            List<string> boats = new List<string>(new string[] { "ferry" });
            lines.Add(new TileLine(this, obj["roads"]["features"], map.parent.tiles, color, boats, 10 * scale));
            //*/

            /*
            JSONObject POIData = obj["pois"]["features"];
            for (int i = 0; i < POIData.Count; i++)
            {
                Debug.Log(POIData[i]);
                pois.Add( new TilePOI( this, POIData[i] ) );
            }
            //*/

        }

    }
}
