import React from 'react'
import PropTypes from 'prop-types'

export const Home = ({ getRebellions, rebellions = [] }) => {
		getRebellions(rebellions)
	return (
		<div>

		</div>
	)
}


Home.propTypes = {
	getRebellions: PropTypes.func,
	rebellions: PropTypes.array,
}
