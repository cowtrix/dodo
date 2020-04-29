import React from "react"
import PropTypes from "prop-types"
import { Button, Selector as SelectorWrapper } from "app/components"
import { Selector } from "./selector"

const title = "Distance..."

export const Distance = ({ latlong, distance, updateDistance }) => (
	<SelectorWrapper
		title={title}
		content={
			<Selector
				placeholder={title}
				distance={distance}
				latlong={latlong}
				updateDistance={updateDistance}
			/>
		}
	/>
)

Distance.propTypes = {
	latlong: PropTypes.string,
	distance: PropTypes.string,
	updateDistance: PropTypes.func
}
