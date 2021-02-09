import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import PropTypes from "prop-types";
import React from "react";

export const Icon = ({ icon, title, className, size = "2x", onClick }) => (
	<FontAwesomeIcon {...{ icon, title, size, className, onClick }} />
);

Icon.propTypes = {
	icon: PropTypes.oneOfType([PropTypes.string, PropTypes.array]),
	title: PropTypes.string,
	className: PropTypes.string,
	onClick: PropTypes.func,
};
