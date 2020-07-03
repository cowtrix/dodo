import React from 'react'
import PropTypes from 'prop-types'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'

export const Icon = ({ icon, title, className, size = "2x", onClick }) =>
	<FontAwesomeIcon
		icon={icon}
		title={title}
		size={size}
		className={className}
		onClick={onClick}
	/>

Icon.propTypes = {
	icon: PropTypes.oneOfType([
		PropTypes.string,
		PropTypes.array,
	]),
	title: PropTypes.string,
	className: PropTypes.string,
}