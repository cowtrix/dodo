import React from 'react'
import PropTypes from 'prop-types'


import styles from './tile.module.scss'


export const Tile = ({ type, resourceTypes = [] }) => {

	console.log(type, resourceTypes)

	const eventColor = resourceTypes.find(resType => type === resType.value).displayColor

	const tileStyles = {
		color: "black",
		backgroundColor: '#' + eventColor
	}

	return (
		<div className={styles.tile} style={tileStyles}>

		</div>
	)
}


Tile.propTypes = {
	event: PropTypes.object,
	resourceTypes: PropTypes.array,
}