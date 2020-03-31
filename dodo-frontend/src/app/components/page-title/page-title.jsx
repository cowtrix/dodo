import React from "react";
import PropTypes from "prop-types";

import styles from "./page-title.module.scss";

export const PageTitle = ({ title, subTitle = null }) => {
	return (
		<div className={styles.PageTitle}>
			<h2 className={styles.title}>{title}</h2>
			{subTitle && <div className={styles.subTitle}>{subTitle}</div>}
		</div>
	);
};

PageTitle.propTypes = {
	title: PropTypes.node.isRequired,
	subTitle: PropTypes.node
};

