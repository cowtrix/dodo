import React from "react"
import { TileLayer } from "react-leaflet"

const TILE_LAYERS = [
	{
		attribution:
			'© <a href="http://osm.org/copyright">OpenStreetMap</a> contributors',
		url: "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
	},
	{
		url:
			"https://maps.rebellion.global/styles/xr_places_non-latin/{z}/{x}/{y}@2x.png"
	}
]

export const TitleLayers = () =>
	TILE_LAYERS.map(tileLayer => (
		<TileLayer key={tileLayer.url} {...tileLayer} />
	))
