import React from 'react'
import PropTypes from 'prop-types'

export const Title = ({ name }) =>
	name ?
		<div>
			<h1>{name}</h1>
		</div> :
		null

Title.propTypes = {
	name: PropTypes.string
}