import React from "react"
import PropTypes from "prop-types"
import { Popup as LeafletPopup } from "react-leaflet"
import { Link } from "react-router-dom"

export const Popup = ({ site }) => (
	<LeafletPopup>
		<Link to={`${"/" + site.metadata.type + "/" + site.slug}`}>
			<h3>{site.name}</h3><h4>{site.metadata.type}</h4>
			</Link>
	</LeafletPopup>
)

Popup.propTypes = {
	site: PropTypes.object
}
