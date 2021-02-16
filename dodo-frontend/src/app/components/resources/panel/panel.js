import React from 'react';
import styles from './panel.module.scss';

export const Panel = ({ title, children = null }) => (
	<div className={styles.container}>
		{title && <h3 className={styles.title}>{title}</h3>}
		{children}
	</div>
);

export default Panel;
