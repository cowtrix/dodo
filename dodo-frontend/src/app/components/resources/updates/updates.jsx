import React from "react";
import PropTypes from "prop-types";
import styles from "./updates.module.scss";
import { useTranslation } from "react-i18next";
import { Panel } from 'app/components/resources';
import { Update } from "./update";

export const Updates = ({ notifications: { notifications = [], nextPageToLoad = false }, loadMore, isLoadingMore = false }) => {
	const { t } = useTranslation("ui");

	return (
		<Panel title={t("notifications_title")}>
			<div className={styles.notifications}>
				{notifications.map((notification) => (
					<Update key={notification.guid} notification={notification} />
				))}
			</div>
			{nextPageToLoad !== false &&
				<div className={styles.loadMore}>
					<button className={styles.loadMoreButton} onClick={loadMore} disabled={isLoadingMore}>
						{isLoadingMore ? t("loading_please_wait") : t("notifications_show_more")}
					</button>
				</div>
			}
		</Panel>
	);
};

Updates.propTypes = {
	notifications: PropTypes.object.isRequired,
};
