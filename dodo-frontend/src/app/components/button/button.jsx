import React from "react"
import PropTypes from "prop-types"

import styles from "./button.module.scss"

export const Button = ({
	children,
	className,
	variant = "primary",
	as = <button />,
	...props
}) => {
	const buttonProps = {
		...props,
		className: `${styles.button} ${styles[variant]} ${className}`
	}

	return React.cloneElement(as, buttonProps, children)
}

Button.propTypes = {
	children: PropTypes.node.isRequired,
	to: PropTypes.string,
	variant: PropTypes.string,
	onClick: PropTypes.func,
	as: PropTypes.node
}
