import React from 'react'
import { withScriptjs, withGoogleMap, GoogleMap, Marker } from "react-google-maps"

export const Map = withScriptjs(withGoogleMap((props) =>
		<GoogleMap
			defaultZoom={8}
			defaultCenter={{ lat: 51.5074, lng: 0.1278 }}
		>
			<Marker position={{ lat: 51.5074, lng: 0.1278 }} />
		</GoogleMap>
	))

