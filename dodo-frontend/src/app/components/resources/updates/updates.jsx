import React from "react";
import PropTypes from "prop-types";
import styles from "./updates.module.scss";
import { useTranslation } from "react-i18next";
import { Update } from "./update";

export const Updates = ({ notifications: { notifications = [], nextPageToLoad = false }, loadMore, isLoadingMore = false }) => {
	const { t } = useTranslation("ui");

	return (
		<div className={styles.container}>
			<h3 className={styles.title}>{t("notifications_title")}</h3>
			<div>
				{notifications.map((notification) => (
					<Update key={notification.guid} notification={notification} />
				))}
			</div>
			{nextPageToLoad === false &&
				<div className={styles.loadMore}>
					<button className={styles.loadMoreButton} onClick={loadMore} disabled={isLoadingMore}>
						{isLoadingMore ? 'Loading, please wait...' : 'See more...'}
					</button>
				</div>
			}
		</div>
	);
};

Updates.propTypes = {
	notifications: PropTypes.object.isRequired,
};
