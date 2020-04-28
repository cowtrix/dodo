import React from "react"
import {
	withScriptjs,
	withGoogleMap,
	GoogleMap,
	Marker
} from "react-google-maps"

const getDefaultCenter = markers => {
	console.trace()
	if (markers && markers.length) {
		return {
			lat: markers[0].latitude,
			lng: markers[0].longitude
		}
	}
	return { lat: 51.5074, lng: 0.1278 }
}

export const Map = withScriptjs(
	withGoogleMap(props => (
		<GoogleMap
			defaultZoom={8}
			defaultCenter={getDefaultCenter(props.markers)}
			options={props.options || {}}
		>
			{props.markers &&
				props.markers.map(marker => (
					<Marker
						position={{
							lat: marker.latitude,
							lng: marker.longitude
						}}
					/>
				))}
		</GoogleMap>
	))
)
