import React from "react";
import { Map } from '../components'

export const RebellionMap = () =>
	<Map
		isMarkerShown
		googleMapURL="https://maps.googleapis.com/maps/api/js?v=3.exp&libraries=geometry,drawing,places"
		loadingElement={<div style={{ height: `100%` }} />}
		containerElement={<div style={{ height: `400px` }} />}
		mapElement={<div style={{ height: `100%` }} />}
	/>