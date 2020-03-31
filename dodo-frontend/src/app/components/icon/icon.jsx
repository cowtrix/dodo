import React from 'react'
import PropTypes from 'prop-types'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'

export const Icon = ({ icon, title, className, size = "2x" }) =>
	<FontAwesomeIcon
		icon={icon}
		title={title}
		size={size}
		className={className}
	/>

Icon.propTypes = {
	icon: PropTypes.oneOfType([
		PropTypes.string,
		PropTypes.array,
	]),
	title: PropTypes.string,
	className: PropTypes.string,
}