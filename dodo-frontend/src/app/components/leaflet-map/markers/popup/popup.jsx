import React from "react"
import PropTypes from "prop-types"
import { Popup as LeafletPopup } from "react-leaflet"
import { Link } from "react-router-dom"

export const Popup = ({ site }) => (
	<LeafletPopup>
		<Link to={`${"/" + site.metadata.type + "/" + site.guid}`}>
			<ul>
				<li>{site.metadata.type}</li>
				<li>{site.name}</li>
				<li>{site.publicDescription}</li>
			</ul>
		</Link>
	</LeafletPopup>
)

Popup.propTypes = {
	site: PropTypes.object
}
