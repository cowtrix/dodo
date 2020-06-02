import React, { Fragment } from 'react'
import PropTypes from 'prop-types'

export const Title = ({ name }) =>
	<div>
		<h1>{name}</h1>
	</div>

Title.propTypes = {
	name: PropTypes.string
}